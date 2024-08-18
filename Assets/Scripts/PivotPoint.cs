using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    public GameObject orbObject;
    private Camera _mainCam;
    private Vector3 _mousePosition;
    public float rotationSpeed = 5f; // Speed at which the pivot rotates

    void Start()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        _mousePosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotateDirection = _mousePosition - transform.position;

        float targetRotationZ = Mathf.Atan2(rotateDirection.y, rotateDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetRotationZ);

        // Interpolate the pivot's rotation towards the target rotation

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        if (transform.localEulerAngles.z > 30 && transform.localEulerAngles.z < 160)
        {
            orbObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {
            orbObject.GetComponent<SpriteRenderer>().sortingOrder = 99;
        }
    }
}