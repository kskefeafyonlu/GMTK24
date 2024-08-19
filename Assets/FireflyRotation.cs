using UnityEngine;

public class Firefly2D : MonoBehaviour
{
    public GameObject centerObject; // The object around which the firefly will move
    public float rotateSpeed = 5f; // Speed of rotation
    public float minRadius = 1f; // Minimum distance from the center object
    public float maxRadius = 3f; // Maximum distance from the center object
    public float randomSpeed = 0.05f; // Speed of random movement
    public float randomRange = 1f; // Range of random movement
    public float radiusChangeSpeed = 1f; // Speed at which the radius changes
    public float smoothSpeed = 0.1f; // Smoothing factor for movement
    public float noiseMagnitude = 0.5f; // Magnitude of the noise (for unpredictable movement)
    public float jitterFrequency = 2f; // Frequency of the jittering

    private Vector3 _position;
    private Vector3 _randomDirection;
    private float _currentRadius;
    private float _radiusChangeDirection;
    private float _noiseOffsetX;
    private float _noiseOffsetY;

    void Start()
    {
        _currentRadius = Random.Range(minRadius, maxRadius);
        _radiusChangeDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
        SetNewRandomDirection();

        // Set random offsets for Perlin noise
        _noiseOffsetX = Random.Range(0f, 100f);
        _noiseOffsetY = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Change the radius over time
        _currentRadius += _radiusChangeDirection * radiusChangeSpeed * Time.deltaTime;

        // Clamp the radius between min and max values and reverse the change direction if needed
        if (_currentRadius > maxRadius || _currentRadius < minRadius)
        {
            _radiusChangeDirection *= -1;
            _currentRadius = Mathf.Clamp(_currentRadius, minRadius, maxRadius);
        }

        // Calculate the new position around the circle with dynamic radius
        _position.x = centerObject.transform.position.x + Mathf.Sin(Time.time * rotateSpeed) * _currentRadius;
        _position.y = centerObject.transform.position.y + Mathf.Cos(Time.time * rotateSpeed) * _currentRadius;

        // Add Perlin noise for smoother randomness
        float noiseX = Mathf.PerlinNoise(Time.time * jitterFrequency + _noiseOffsetX, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, Time.time * jitterFrequency + _noiseOffsetY) - 0.5f;

        // Apply noise to the firefly's position for unpredictable movement
        _position.x += noiseX * noiseMagnitude;
        _position.y += noiseY * noiseMagnitude;

        // Smoothly move the firefly towards the calculated position
        transform.position = Vector3.Lerp(transform.position, _position, smoothSpeed);

        // Random movement within the random range
        if (Vector3.Distance(transform.position, centerObject.transform.position) < randomRange)
        {
            transform.position += _randomDirection * randomSpeed * Time.deltaTime;
        }
        else
        {
            SetNewRandomDirection();
        }
    }

    private void SetNewRandomDirection()
    {
        _randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
