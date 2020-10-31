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

    [Header("Floats")]
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float horizontal;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float jetpackFuel;
    [SerializeField]
    private float jetpackPower;

    [Header("Bools")]
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool jetpackOn;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jetpackOn = false;
        jetpack.gameObject.SetActive(jetpackOn);
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.right * horizontal * speed);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            if (!jetpackOn)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.Space) && jetpackOn)
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 1f;

            float distance = Mathf.Abs(collision.GetComponent<GravityPoint>().PlanetRadius - Vector2.Distance(transform.position, collision.transform.position));
            //Debug.Log(distance);

            isGrounded = distance < 0.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 0.2f;
        }
    }
}
