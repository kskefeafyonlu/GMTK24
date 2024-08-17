using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScaleGun : MonoBehaviour
{
    public Material lineMaterial;
    public HoldableObject hitHoldableObject;
    public Color holdColor = Color.red; // Color when holding the mouse button
    public float SlerpSpeed = 5f; // Speed of the Slerp movement

    private LineRenderer _lineRenderer;
    private Color _originalColor = Color.white;
    private Color _originalColor2 = Color.cyan;
    private float _normalizedDistance;
    private bool _isHoldingObject = false;
    private bool _isInHoldMode = false;

    private void Awake()
    {
        InitializeLineRenderer();
    }

    private void Update()
    {
        DrawLineUpwards();
        CheckForHoldableObject();
        HoldLogic();
        ModifyHoldableObject();
        
        DrawSplineBetweenObjectAndGun();
    }

    private void HoldLogic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isInHoldMode = true;
        }

        if (Input.GetMouseButton(0) && _isInHoldMode)
        {
            if (hitHoldableObject != null && !_isHoldingObject)
            {
                StartHoldingObject();
            }

            if (_isHoldingObject)
            {
                ControlHoldableObject();
            }

            SetLineColor(holdColor);
        }
        else
        {
            SetLineColor(_originalColor, _originalColor2);
            _isHoldingObject = false;
            _isInHoldMode = false;
            ResetLineRenderer(); // Reset the LineRenderer when the mouse button is released
        }
    }

    // Initialize the LineRenderer component
    private void InitializeLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = lineMaterial;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 2;
        _originalColor = _lineRenderer.startColor;
    }

    // Draw a line upwards from the object's position
    private void DrawLineUpwards()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.up; // Use the parent's up direction
        Vector3 endPosition = startPosition + direction * 10; // Adjust the length as needed

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    // Check if there is a holdable object in the line's path
    private void CheckForHoldableObject()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 10); // Adjust the length as needed

        if (hit.collider != null)
        {
            HoldableObject holdableObject = hit.collider.GetComponent<HoldableObject>();

            if (holdableObject != null)
            {
                Debug.Log("Hit " + holdableObject.name);
                hitHoldableObject = holdableObject;
            }
            else
            {

                
            }
        }
        else
        {
            if (!_isInHoldMode)
            {
                hitHoldableObject = null;
            }
        }
    }

    // Start holding the object and calculate the initial normalized distance
    private void StartHoldingObject()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, hitHoldableObject.transform.position);
        float maxDistance = Vector3.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1));
        _normalizedDistance = Mathf.Clamp01(distanceToPlayer / maxDistance);
        _isHoldingObject = true;
    }

    // Control the position of the holdable object along the line
    private void ControlHoldableObject()
    {
        if (hitHoldableObject == null) return;

        Vector3 startPosition = _lineRenderer.GetPosition(0);
        Vector3 endPosition = _lineRenderer.GetPosition(1);

        // Project the normalized distance onto the line
        Vector3 lineDirection = (endPosition - startPosition).normalized;
        Vector3 pointOnLine = startPosition + lineDirection * (_normalizedDistance * Vector3.Distance(startPosition, endPosition));

        // Move the holdable object to the calculated point on the line using Slerp
        hitHoldableObject.transform.position = Vector3.Slerp(hitHoldableObject.transform.position, pointOnLine, Time.deltaTime * SlerpSpeed / hitHoldableObject.Mass); // Adjust the speed factor as needed
    }

    // Set the color of the line
    private void SetLineColor(Color startColor, Color endColor)
    {
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

    // Overload method to set the same color for both start and end of the line
    private void SetLineColor(Color color)
    {
        SetLineColor(color, color);
    }

    // Modify the scale of the holdable object
    public float scaleChangeSpeed = 0.1f; // Speed of the scale change

    private void ModifyHoldableObject()
    {
        if (hitHoldableObject == null) return;

        Vector3 scaleChange = new Vector3(scaleChangeSpeed, scaleChangeSpeed, scaleChangeSpeed) * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q) && hitHoldableObject.transform.localScale.x < hitHoldableObject.MaxScale)
        {
            hitHoldableObject.transform.localScale += scaleChange;
            hitHoldableObject.Mass *= 1 + scaleChange.x;
        }
        else if (Input.GetKey(KeyCode.E) && hitHoldableObject.transform.localScale.x > hitHoldableObject.MinScale)
        {
            hitHoldableObject.transform.localScale -= scaleChange;
            hitHoldableObject.Mass *= 1 - scaleChange.x;
        }
    }


    private void DrawSplineBetweenObjectAndGun()
    {
        if (hitHoldableObject == null || !_isInHoldMode) return;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = hitHoldableObject.transform.position;
        Vector3 controlPoint = startPosition + transform.up * 5; // Adjust the control point to be upwards from the gun

        int numPoints = 20; // Number of points on the spline
        _lineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 pointOnSpline = Mathf.Pow(1 - t, 2) * startPosition + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPosition;
            _lineRenderer.SetPosition(i, pointOnSpline);
        }
    }
    
    private void ResetLineRenderer()
    {
        _lineRenderer.positionCount = 2;
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.up; // Use the parent's up direction
        Vector3 endPosition = startPosition + direction * 10; // Adjust the length as needed

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }
}