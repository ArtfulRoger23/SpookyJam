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

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            if (!jetpackOn)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                anim.SetTrigger("jump");
            }
        }

        if (Input.GetKey(KeyCode.Space) && jetpackOn && jetpackFuel > 0)
        {
            rb.AddForce(transform.up * 4, ForceMode2D.Force);
            jetpackFuel -= 1 * Time.deltaTime;
            jetpack.startSpeed = -5f;
        }
        else
        {
            jetpack.startSpeed = -2f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            jetpackOn = !jetpackOn;
            jetpack.gameObject.SetActive(jetpackOn);
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

            if (p != null)
            {
                switch (p.PickUpType)
                {
                    case "Fuel":
                        jetpackFuel += 50;
                        break;
                }
            }

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            if (!reachedGoal)
            {
                reachedGoal = true;
                StartCoroutine(FinishLevel());
            }
        }
        else if (collision.gameObject.CompareTag("Border"))
        {
            Death();
        }
    }

    private IEnumerator FinishLevel()
    {
        float time = Time.realtimeSinceStartup;
        float timer = 0.0f;

        while (timer < 1.0f)
        {
            Time.timeScale = Mathf.Lerp(1.0f, 0.0f, timer / 1.0f);
            timer += (Time.realtimeSinceStartup - time);
            time = Time.realtimeSinceStartup;

            yield return null;
        }

        Time.timeScale = 0.0f;

        SceneManager.LoadScene(2);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 1f;

            float distance = Mathf.Abs(collision.GetComponent<GravityPoint>().PlanetRadius - Vector2.Distance(transform.position, collision.transform.position));
            //Debug.Log(distance);

            isGrounded = distance < 0.6f;

            isInGravity = true;
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            canPlaceFlag = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Checkpoint cp = collision.gameObject.GetComponent<Checkpoint>();

                Debug.Log("Pressed E");

                if (!cp.Activated)
                {
                    Debug.Log("Activate");
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
