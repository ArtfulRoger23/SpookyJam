using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    [SerializeField]
    private float gravityScale;
    [SerializeField]
    private float planetRadius;
    [SerializeField]
    private float gravityMinRange;
    [SerializeField]
    private float gravityMaxRange;

    public float PlanetRadius { get => planetRadius; }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, planetRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        float gravitationalPower = gravityScale;
        float dist = Vector2.Distance(collision.transform.position, transform.position);

        if (dist > (planetRadius + gravityMinRange))
        {
            float min = planetRadius + gravityMinRange;
            gravitationalPower = (((min + gravityMaxRange) - dist) / gravityMaxRange) * gravitationalPower;
        }

        Vector3 dir = (transform.position - collision.transform.position) * gravitationalPower;

        collision.GetComponent<Rigidbody2D>().AddForce(dir);

        if (collision.CompareTag("Player"))
        {
            collision.transform.up = Vector3.MoveTowards(collision.transform.up, -dir, gravityScale * Time.deltaTime);
        }
    }
}
