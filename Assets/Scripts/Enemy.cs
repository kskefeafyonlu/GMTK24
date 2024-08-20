using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public float minSpeed = 1f; // Minimum speed for this enemy type
    public float maxSpeed = 1.5f; // Maximum speed for this enemy type
    private float movementSpeed; // Actual speed for this enemy instance

    public float maxHealth = 100f;
    public float currentHealth;
    private Slider _healthBar;

    public int damagePerSecond = 10; // Damage to apply per second
    private float _damageTimer;

    private float _slowdownDuration = 1f; // Duration of the slowdown effect
    private float _slowdownFactor = 0.5f; // Factor by which the speed is reduced

    public GameObject floatingTextPrefab;
    private void Awake()
    {
        floatingTextPrefab = Resources.Load<GameObject>("FloatingText");
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _healthBar = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;
        movementSpeed = Random.Range(minSpeed, maxSpeed); // Assign a random speed within the range
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

        // Flip the sprite based on the enemy's position relative to the player
        if (_target.position.x > transform.position.x)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
           _spriteRenderer.flipX = true;
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            _animator.SetTrigger("OnHit");
            StartCoroutine(SlowdownEffect());
        }
        UpdateUI();
        ShowFloatingText(damage);
    }
    
// Enemy.cs
    private void ShowFloatingText(float damage)
    {
        if (floatingTextPrefab != null)
        {
            GameObject floatingTextInstance = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText floatingText = floatingTextInstance.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.Initialize(damage);
            }
        }
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
                _animator.SetTrigger("Attack"); // Trigger attack animation
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
                    _animator.SetTrigger("Attack"); // Trigger attack animation
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
            
            holdableObject.HP--;
            if (holdableObject.HP <= 0)
            {
                Destroy(other.gameObject);
            }
        }
    }

    private float CalculateDamage(float mass, float velocity)
    {
        return mass * velocity * velocity * 0.5f;
    }

    private IEnumerator SlowdownEffect()
    {
        movementSpeed *= _slowdownFactor;
        yield return new WaitForSeconds(_slowdownDuration);
        movementSpeed /= _slowdownFactor;
    }
}