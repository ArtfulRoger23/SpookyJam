using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private string pickUpType;

    public string PickUpType { get => pickUpType; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * rotateSpeed * Time.deltaTime);
    }
}
