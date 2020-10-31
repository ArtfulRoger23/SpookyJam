using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private string pickUpType;
    [SerializeField]
    private AudioSource pickUpSound;
    [SerializeField]
    private bool pickedUp;

    public string PickUpType { get => pickUpType; }
    public bool PickedUp { get => pickedUp; set => pickedUp = value; }


    // Start is called before the first frame update
    void Start()
    {
        pickedUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * rotateSpeed * Time.deltaTime);

        if (pickedUp && !pickUpSound.isPlaying)
            Destroy(gameObject);
    }

    public void PickedUpItem()
    {
        if (pickedUp)
            return;

        pickedUp = true;

        if (!pickUpSound.isPlaying)
            pickUpSound.Play();
    }
}
