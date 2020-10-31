using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jetpack.gameObject.SetActive(jetpackOn);
        anim = GetComponent<Animator>();
        jetpackOn = false;
        jetpackFuel = 100;
    }

    private void FixedUpdate()
    {
        if (isInGravity)
            rb.AddForce(transform.right * horizontal * speed);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 0.2f;

            isInGravity = false;
        }
    }
}
