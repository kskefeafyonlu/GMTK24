using UnityEngine;

public class ScaleGun : MonoBehaviour
{
    public Upgrades upgrades;

    public PlayerMovement playerMovement;
    private Camera _mainCam;

    public Material lineMaterial;
    public Material splineMaterial;
    public HoldableObject hitHoldableObject;
    
    public float slerpSpeed = 5f;
    public float initialLength;
    public float maxLineLength = 10f;
    public float scaleChangeSpeed = 1f;
    public float lengthGrowSpeed = 5f;
    public float shootForceSpeed = 5f;

    private LineRenderer _lineRenderer;
    private LineRenderer _splineRenderer;
    private GameObject _splineObject;
    private Color _originalColorStart = Color.white;
    private readonly Color _originalColorEnd = Color.red;
    private bool _isHoldingObject;
    private bool _isInHoldMode;
    private float _currentLineLength;
    private Vector3 _lastPosition;
    private Vector3 _objectVelocity;
    private float throwForce;


    private float _initialDistanceToPlayer;

    public GameObject objectPrefab;

    private void Awake()
    {
        _mainCam = Camera.main;
        playerMovement = GetComponentInParent<PlayerMovement>();

        InitializeLineRenderer();
        InitializeSplineRenderer();
        _currentLineLength = initialLength;
    }

    private void Start()
    {
        UpdateUpgradeValues();
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
        
        UpdateUpgradeValues();
    }

    private void Shoot()
{
    if (hitHoldableObject != null)
    {
        _isHoldingObject = false;
        _isInHoldMode = false;

        Vector3 mousePosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - hitHoldableObject.transform.position).normalized;

        Rigidbody2D rb = hitHoldableObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            throwForce = upgrades.UpgradesUI[2].Points * shootForceSpeed; // Use upgrade points for throw force
            rb.velocity = direction * throwForce * _objectVelocity.magnitude;
        }

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
        _originalColorStart = _lineRenderer.startColor;

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

    private void CheckForHoldableObject()
    {
        if (_isHoldingObject) return;

        int holdableLayerMask = LayerMask.GetMask("HoldableObject");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, _currentLineLength, holdableLayerMask);

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
                if (hitHoldableObject.isCampfire)
                {
                    // Skip movement logic for campfire
                    ModifyHoldableObject();
                }
                else
                {
                    ControlHoldableObject();
                    ModifyHoldableObject();
                }
            }

            IncreaseLineLength();
            SetLineColor(Color.white, Color.cyan);
        }
        else
        {
            SetLineColor(_originalColorStart, _originalColorEnd);
            _isHoldingObject = false;
            _isInHoldMode = false;
            DecreaseLineLength();
            ResetLineRenderer();
            DisableSplineRenderer();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
            ResetLineRenderer();
            DisableSplineRenderer();
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
        _isHoldingObject = true;
        _lastPosition = hitHoldableObject.transform.position;
    }


    private float affectingSlerpSpeed = 1;
    private void ControlHoldableObject()
    {
        if (hitHoldableObject == null) return;

        Vector3 startPosition = _lineRenderer.GetPosition(0);
        Vector3 endPosition = startPosition + transform.right * _initialDistanceToPlayer;

        hitHoldableObject.transform.position = Vector3.Lerp(hitHoldableObject.transform.position, endPosition,
            Time.deltaTime * slerpSpeed / hitHoldableObject.Mass);

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
            hitHoldableObject.transform.localScale += scaleChange * upgrades.UpgradesUI[3].Points; // Use upgrade points for scaling amount
            hitHoldableObject.Mass *= 1 + scaleChange.x;
            _lineRenderer.startColor = Color.green;
        }
        else if (Input.GetKey(KeyCode.E) && hitHoldableObject.transform.localScale.x > hitHoldableObject.MinScale)
        {
            hitHoldableObject.transform.localScale -= scaleChange * upgrades.UpgradesUI[3].Points; // Use upgrade points for scaling amount
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

        DisableLineRenderer();

        Vector3 startPosition = transform.position;
        Vector3 endPosition = hitHoldableObject.transform.position;
        Vector3 controlPoint = startPosition + transform.right * 5;

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

    
    private float affectingMaxLineLength = 1;
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
        EnableLineRenderer();
    }

    private void UpdateUpgradeValues()
    {
        
        affectingSlerpSpeed = upgrades.UpgradesUI[0].Points * slerpSpeed;
        affectingMaxLineLength = upgrades.UpgradesUI[1].Points * maxLineLength;
        scaleChangeSpeed = upgrades.UpgradesUI[3].Points;
        
    }
}