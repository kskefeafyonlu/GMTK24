using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleGun : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        DrawLineUpwards();
    }

    private void DrawLineUpwards()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.up; // Use the parent's up direction
        Vector3 endPosition = startPosition + direction * 10; // Adjust the length as needed

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}