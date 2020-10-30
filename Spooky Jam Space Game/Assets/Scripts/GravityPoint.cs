using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    [SerializeField]
    private float gravityScale;

    [SerializeField]
    private float planetRadius;

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
        Vector3 dir = (transform.position - collision.transform.position) * gravityScale;

        collision.GetComponent<Rigidbody2D>().AddForce(dir);

        if (collision.CompareTag("Player"))
        {
            collision.transform.up = Vector3.MoveTowards(collision.transform.up, -dir, gravityScale * Time.deltaTime);
        }
    }
}
