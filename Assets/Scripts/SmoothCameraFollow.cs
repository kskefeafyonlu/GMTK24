// SmoothCameraFollow.cs
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target; // The target for the camera to follow
    public float smoothSpeed = 0.125f; // The smoothness of the camera movement
    public Vector3 offset; // Offset from the target position

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }
}