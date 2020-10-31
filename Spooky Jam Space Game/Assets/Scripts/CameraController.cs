using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject mapLight;

    [Header("Transforms")]
    [SerializeField]
    private Transform target;

    [Header("Vector2s")]
    [SerializeField]
    private Vector2 velocity;

    [Header("Vector3s")]
    [SerializeField]
    private Vector3 minCameraPos;
    [SerializeField]
    private Vector3 maxCameraPos;
    [SerializeField]
    private Vector3 mapCenter;

    [Header("Quaternions")]
    [SerializeField]
    private Quaternion defaultRotation;

    [Header("Bools")]
    [SerializeField]
    private bool bounds;
    [SerializeField]
    private bool rotateWithTarget = false;
    [SerializeField]
    private bool viewMap = false;

    [Header("Floats")]
    [SerializeField]
    private float smoothTimeX;
    [SerializeField]
    private float smoothTimeY;
    [SerializeField]
    private float defaultSize;
    [SerializeField]
    private float mapSize;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        defaultRotation = transform.rotation;
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
        mapLight.SetActive(false);

        //rotateWithTarget = (PlayerPrefs.GetInt("RotateWithPlayer") == 1) ? true : false;
    }

    private void FixedUpdate()
    {
        if (!viewMap)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(posX, posY, transform.position.z);

            if (rotateWithTarget)
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * 5f);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, Time.deltaTime * 5f);

            if (bounds)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y), Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * 5f);
        }
        else
        {
            float posX = Mathf.SmoothDamp(transform.position.x, mapCenter.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, mapCenter.y, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(posX, posY, transform.position.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, Time.deltaTime * 5f);

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, mapSize, Time.deltaTime * 5f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            viewMap = !viewMap;
            mapLight.SetActive(viewMap);
        }
    }
}
