using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScaleGun : MonoBehaviour
{
    public Material lineMaterial;
    public HoldableObject hitHoldableObject;
    public float SlerpSpeed = 5f; // Speed of the Slerp movement
    public float initialLength = 0f; // Initial length when holding starts
    public float lengthChangeSpeed = 1f; // Speed of the length change
    public float maxLineLength = 10f; // Maximum length of the line
    public float scaleChangeSpeed = 1f; // Speed of the scale change
    public float lengthGrowSpeed = 5f; // Speed of the length growth

    private LineRenderer _lineRenderer;
    private Color _originalColor = Color.white;
    private Color _originalColor2 = Color.red;
    private float _normalizedDistance;
    private bool _isHoldingObject = false;
    private bool _isInHoldMode = false;
    private float _currentLineLength;
    private Vector3 _lastPosition;
    private Vector3 _objectVelocity;

    private void Awake()
    {
        InitializeLineRenderer();
        _currentLineLength = initialLength; // Set initial length to 0
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

            IncreaseLineLength();
            SetColor(Color.white, Color.cyan);
        }
        else
        {
            SetLineColor(_originalColor, _originalColor2);
            _isHoldingObject = false;
            _isInHoldMode = false;
            DecreaseLineLength(); // Decrease the LineRenderer length when the mouse button is released
            if (hitHoldableObject != null)
            {
                ApplyMomentum();
                ResetLineRenderer();
            }
        }
    }

    // Gradually increase the length of the line
    private void IncreaseLineLength()
    {
        _currentLineLength = Mathf.Min(_currentLineLength + lengthGrowSpeed * Time.deltaTime, maxLineLength);
    }

    // Gradually decrease the length of the line
    private void DecreaseLineLength()
    {
        _currentLineLength = Mathf.Max(_currentLineLength - lengthGrowSpeed * Time.deltaTime, 0f);
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
        Vector3 endPosition = startPosition + direction * _currentLineLength; // Adjust the length as needed

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    // Check if there is a holdable object in the line's path
    private void CheckForHoldableObject()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _currentLineLength); // Adjust the length as needed

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
                hitHoldableObject = null;
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
        _lastPosition = hitHoldableObject.transform.position;
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

        // Calculate the velocity of the object
        _objectVelocity = (hitHoldableObject.transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = hitHoldableObject.transform.position;
    }

    // Apply the stored velocity to the object when it is released
    private void ApplyMomentum()
    {
        if (hitHoldableObject != null)
        {
            Rigidbody2D rb = hitHoldableObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = _objectVelocity;
            }
        }
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
    private void ModifyHoldableObject()
    {
        if (hitHoldableObject == null) return;

        Vector3 scaleChange = new Vector3(scaleChangeSpeed, scaleChangeSpeed, scaleChangeSpeed) * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q) && hitHoldableObject.transform.localScale.x < hitHoldableObject.MaxScale)
        {
            hitHoldableObject.transform.localScale += scaleChange;
            hitHoldableObject.Mass *= 1 + scaleChange.x;
            _lineRenderer.startColor = Color.green;
        }
        else if (Input.GetKey(KeyCode.E) && hitHoldableObject.transform.localScale.x > hitHoldableObject.MinScale)
        {
            hitHoldableObject.transform.localScale -= scaleChange;
            hitHoldableObject.Mass *= 1 - scaleChange.x;
            _lineRenderer.startColor = Color.red;
        }
        else
        {
            _lineRenderer.startColor = Color.white;
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
        Vector3 endPosition = startPosition + direction * initialLength; // Adjust the length as needed

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
        _currentLineLength = initialLength;
    }

    void SetColor(Color startColor, Color endColor)
    {
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }
}