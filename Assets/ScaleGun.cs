using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScaleGun : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public HoldableObject HitHoldableObject;
    

    private void Awake()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        DrawLineUpwards();
        CheckForHoldableObject();

        if (Input.GetMouseButton(0))
        {
            ControlHoldableObject();
        }
    }

    private void DrawLineUpwards()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.up; // Use the parent's up direction
        Vector3 endPosition = startPosition + direction * 10; // Adjust the length as needed

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    private void CheckForHoldableObject()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 10); // Adjust the length as needed

        if (hit.collider != null)
        {
            HoldableObject holdableObject = hit.collider.GetComponent<HoldableObject>();

            if (holdableObject != null)
            {
                Debug.Log(" Hit " + holdableObject.name);
                HitHoldableObject = holdableObject;
            }
            else
            {
                HitHoldableObject = null;
            }
        }
        else
        {
            HitHoldableObject = null;
        }
    }
    
    private void ControlHoldableObject()
    {
        if (HitHoldableObject != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Ensure the z position is set to 0
            HitHoldableObject.transform.position = mouseWorldPos;
        }
    }
}