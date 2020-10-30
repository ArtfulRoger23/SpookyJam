using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float horizontal;
    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            rb.drag = 1f;

            float distance = Mathf.Abs(collision.GetComponent<GravityPoint>().PlanetRadius - Vector2.Distance(transform.position, collision.transform.position));
            Debug.Log(distance);

            isGrounded = distance < 0.5f;

            //if (distance < 0.5f)
            //{
            //    isGrounded = distance < 0.5f;
            //}
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
