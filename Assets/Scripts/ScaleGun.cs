using UnityEngine;
public class ScaleGun : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    
    public Material lineMaterial;
    public Material splineMaterial;
    public HoldableObject hitHoldableObject;
    public float SlerpSpeed = 5f;
    public float initialLength = 0f;
    public float lengthChangeSpeed = 1f;
    public float maxLineLength = 10f;
    public float scaleChangeSpeed = 1f;
    public float lengthGrowSpeed = 5f;

    private LineRenderer _lineRenderer;
    private LineRenderer _splineRenderer;
    private GameObject _splineObject;
    private Color _originalColor = Color.white;
    private Color _originalColor2 = Color.red;
    private float _normalizedDistance;
    private bool _isHoldingObject = false;
    private bool _isInHoldMode = false;
    private float _currentLineLength;
    private Vector3 _lastPosition;
    private Vector3 _objectVelocity;
    private Camera mainCam;
    
    private bool isGunFacingRight = true;

    private float _initialDistanceToPlayer;

    public GameObject objectPrefab;

    private void Awake()
    {
        PlayerMovement = GetComponentInParent<PlayerMovement>();
        
        mainCam = Camera.main;
        InitializeLineRenderer();
        InitializeSplineRenderer();
        _currentLineLength = initialLength;
    }

    private void Update()
    { 
        DrawLineToRight();
        CheckForHoldableObject();
        HandleHoldLogic();
        ModifyHoldableObject();
        DrawSplineBetweenObjectAndGun();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(objectPrefab, transform.position + transform.right * 2, Quaternion.identity);
        }
    }
    private void Shoot()
    {
        if (hitHoldableObject != null)
        {
            // Release the object
            _isHoldingObject = false;
            _isInHoldMode = false;

            // Apply shooting logic
            Rigidbody2D rb = hitHoldableObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.right * 10f; // Set bullet speed
            }

            // Clear the reference to the holdable object
            hitHoldableObject = null;
        }
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = lineMaterial;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 2;
        _originalColor = _lineRenderer.startColor;
        
        _lineRenderer.transform.SetParent(transform);
    }

    private void InitializeSplineRenderer()
    {
        _splineObject = new GameObject("SplineRenderer");
        _splineObject.transform.SetParent(transform);
        _splineRenderer = _splineObject.AddComponent<LineRenderer>();
        _splineRenderer.material = splineMaterial;
        _splineRenderer.startWidth = 0.1f;
        _splineRenderer.endWidth = 0.1f;
        _splineRenderer.positionCount = 0;
    }
    private void DrawLineToRight()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.right;
        Vector3 endPosition = startPosition + direction * _currentLineLength;

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }
    
    private void DrawLineToLeft()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = -transform.right; // Change direction to left
        Vector3 endPosition = startPosition + direction * _currentLineLength;

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    private void CheckForHoldableObject()
    {
        if (_isHoldingObject) return; // Prevent picking up another object if one is already held

        int holdableLayerMask = LayerMask.GetMask("HoldableObject");
        RaycastHit2D hit =
            Physics2D.Raycast(transform.position, transform.right, _currentLineLength, holdableLayerMask);

        if (hit.collider != null)
        {
            HoldableObject holdableObject = hit.collider.GetComponent<HoldableObject>();
            hitHoldableObject = holdableObject != null ? holdableObject : null;
        }
        else if (!_isInHoldMode)
        {
            hitHoldableObject = null;
        }
    }

    private void HandleHoldLogic()
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
            SetLineColor(Color.white, Color.cyan); // Set the line color to cyan when holding an object
        }
        else
        {
            SetLineColor(_originalColor, _originalColor2);
            _isHoldingObject = false;
            _isInHoldMode = false;
            DecreaseLineLength();
            if (hitHoldableObject != null)
            {
                ApplyMomentum();
                ResetLineRenderer();
                DisableSplineRenderer();
                Shoot(); // Call Shoot method when left mouse button is released
            }
        }
    }

    private void DisableLineRenderer()
    {
        _lineRenderer.enabled = false;
    }

    private void EnableLineRenderer()
    {
        _lineRenderer.enabled = true;
    }

    private void StartHoldingObject()
    {
        _initialDistanceToPlayer = Vector3.Distance(transform.position, hitHoldableObject.transform.position);
        float maxDistance = Vector3.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1));
        _normalizedDistance = Mathf.Clamp01(_initialDistanceToPlayer / maxDistance);
        _isHoldingObject = true;
        _lastPosition = hitHoldableObject.transform.position;
    }

    private void ControlHoldableObject()
    {
        if (hitHoldableObject == null) return;

        Vector3 startPosition = _lineRenderer.GetPosition(0);
        Vector3 endPosition = startPosition + transform.right * _initialDistanceToPlayer; // Change direction to right

        hitHoldableObject.transform.position = Vector3.Lerp(hitHoldableObject.transform.position, endPosition,
            Time.deltaTime * SlerpSpeed / hitHoldableObject.Mass);

        _objectVelocity = (hitHoldableObject.transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = hitHoldableObject.transform.position;
    }

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

    private void SetLineColor(Color startColor, Color endColor)
    {
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

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

        DisableLineRenderer(); // Disable the line renderer when the spline is active

        Vector3 startPosition = transform.position;
        Vector3 endPosition = hitHoldableObject.transform.position;
        Vector3 controlPoint = startPosition + transform.right * 5; // Change direction to right

        int numPoints = 20;
        _splineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 pointOnSpline = Mathf.Pow(1 - t, 2) * startPosition + 2 * (1 - t) * t * controlPoint +
                                    Mathf.Pow(t, 2) * endPosition;
            _splineRenderer.SetPosition(i, pointOnSpline);
        }
    }



    private void ResetLineRenderer()
    {
        _lineRenderer.positionCount = 2;
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.up;
        Vector3 endPosition = startPosition + direction * initialLength;

        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
        _currentLineLength = initialLength;
    }

    private void IncreaseLineLength()
    {
        _currentLineLength = Mathf.Min(_currentLineLength + lengthGrowSpeed * Time.deltaTime, maxLineLength);
    }

    private void DecreaseLineLength()
    {
        _currentLineLength = Mathf.Max(_currentLineLength - lengthGrowSpeed * Time.deltaTime, 0f);
    }

    private void DisableSplineRenderer()
    {
        _splineRenderer.positionCount = 0;
        EnableLineRenderer(); // Re-enable the line renderer when the spline is disabled
    }
}