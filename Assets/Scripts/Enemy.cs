using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private Rigidbody2D _rb;

    public float movementSpeed = 1f;
    public float maxHealth = 100f;
    public float currentHealth;
    private Slider _healthBar;

    public int damagePerSecond = 10; // Damage to apply per second
    private float _damageTimer;

    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _healthBar = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void FixedUpdate()
    {
        PathFind();
    }

    private void PathFind()
    {
        Vector2 direction = (Vector2)_target.position - _rb.position;
        direction.Normalize();
        _rb.velocity = direction * movementSpeed;
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateUI();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        _healthBar.value = currentHealth / maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit by player");
            if (other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(10);
            }
        }
        else if (other.gameObject.CompareTag("HoldableObject"))
        {
            Debug.Log("Hit by holdable object");
            CalculateDamageFromObject(other);
            SlowDownObject(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                _damageTimer += Time.deltaTime;
                if (_damageTimer >= 1f)
                {
                    playerHealth.TakeDamage(damagePerSecond);
                    _damageTimer = 0f;
                }
            }
        }
    }

    private void SlowDownObject(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.velocity *= 0.7f; // Reduce the velocity by half
        }
    }

    private void CalculateDamageFromObject(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<HoldableObject>(out var holdableObject) && other.attachedRigidbody != null)
        {
            Debug.Log("Calculating damage");
            float damage = CalculateDamage(holdableObject.Mass, other.attachedRigidbody.velocity.magnitude);
            TakeDamage(damage);
        }
    }

    private float CalculateDamage(float mass, float velocity)
    {
        return mass * velocity * velocity * 0.5f;
    }
}