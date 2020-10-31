using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private ParticleSystem jetpack;
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private AudioSource fuelBoost;
    [SerializeField]
    private AudioSource planetCollide;

    [SerializeField]
    private AudioClip useFuel;
    [SerializeField]
    private AudioClip noFuel;

    [Header("Floats")]
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float horizontal;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    [Range(0, 100)]
    private float jetpackFuel;
    [SerializeField]
    private float jetpackPower;
    [SerializeField]
    private float rotateSpeed;

    [Header("Bools")]
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool jetpackOn;
    [SerializeField]
    private bool isInGravity;
    [SerializeField]
    private bool reachedGoal;
    [SerializeField]
    private bool canPlaceFlag;
    [SerializeField]
    private bool canJetpack;

    [Header("GameObjects")]
    private Checkpoint lastCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jetpack.gameObject.SetActive(jetpackOn);
        anim = GetComponent<Animator>();
        jetpackOn = false;
        jetpackFuel = 100;
        reachedGoal = false;
        canPlaceFlag = false;
    }

    private void FixedUpdate()
    {
        if (reachedGoal)
            return;

        if (isInGravity)
            rb.AddForce(transform.right * horizontal * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (reachedGoal)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");

        if (isInGravity)
        {
            anim.SetFloat("speed", Mathf.Abs(horizontal));

            if (horizontal != 0)
                transform.localScale = new Vector3(Mathf.Clamp(horizontal, -0.5f, 0.5f), 0.5f, 0.5f);
        }
        else
        {
            anim.SetFloat("speed", 0f);
        }

        if (isGrounded)
            canJetpack = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            anim.SetTrigger("jump");
        }
        else if (Input.GetButtonUp("Jump") && !isGrounded)
        {
            canJetpack = true;
        }

        if (Input.GetKey(KeyCode.Space) && canJetpack && jetpackFuel > 0)
        {
            rb.AddForce(transform.up * 4, ForceMode2D.Force);
            jetpackFuel -= 1 * Time.deltaTime;
            jetpack.gameObject.SetActive(true);

            if (!fuelBoost.isPlaying)
            {
                fuelBoost.clip = useFuel;
                fuelBoost.Play();
            }
        }
        else if (Input.GetKey(KeyCode.Space) && canJetpack && jetpackFuel <= 0)
        {
            if (!fuelBoost.isPlaying)
            {
                fuelBoost.clip = noFuel;
                fuelBoost.Play();
            }
        }
        else
        {
            jetpack.gameObject.SetActive(false);

            if (fuelBoost.isPlaying)
            {
                fuelBoost.Stop();
            }
        }

        if (!isInGravity)
        {
            Quaternion targetRotation = transform.rotation;
            float angle = transform.rotation.z;

            if (Input.GetKey(KeyCode.RightArrow))
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1 * rotateSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.LeftArrow))
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 1 * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            PickUp p = collision.gameObject.GetComponent<PickUp>();

            if (p.PickedUp)
                return;

            if (p != null)
            {
                switch (p.PickUpType)
                {
                    case "Fuel":
                        jetpackFuel += 50;
                        break;
                }
            }

            p.PickedUpItem();
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            VictoryPoint v = collision.gameObject.GetComponent<VictoryPoint>();

            if (v.Activated)
                return;

            v.Activate();
        }
        else if (collision.gameObject.CompareTag("Border"))
        {
            Death();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 1f;

            float distance = Mathf.Abs(collision.GetComponent<GravityPoint>().PlanetRadius - Vector2.Distance(transform.position, collision.transform.position));
            //Debug.Log(distance);

            bool temp = isGrounded;

            isGrounded = distance < 0.6f;
            
            if (temp == false && isGrounded == true)
            {

            }

            isInGravity = true;
            anim.SetBool("floatIdle", false);
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            canPlaceFlag = true;

            Checkpoint cp = collision.gameObject.GetComponent<Checkpoint>();

            if (!cp.Activated)
            {
                if (!cp.ClaimPrompt.activeSelf)
                    cp.ClaimPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    cp.Activate();
                    lastCheckpoint = cp;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 0.2f;

            isInGravity = false;
            anim.SetBool("floatIdle", true);
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            canPlaceFlag = false;
        }
    }

    private void Death()
    {
        rb.velocity = Vector2.zero;

        if (lastCheckpoint != null)
            transform.position = lastCheckpoint.gameObject.transform.position;
        else
        {
            GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");

            if (respawn != null)
            {
                transform.position = respawn.transform.position;
            }
        }
    }
}
