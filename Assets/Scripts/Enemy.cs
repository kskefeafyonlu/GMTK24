using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public float minSpeed = 1f;
    public float maxSpeed = 1.5f;
    private float movementSpeed;

    public float maxHealth = 100f;
    public float currentHealth;
    private Slider _healthBar;

    public int damagePerSecond = 10;
    private float _damageTimer;

    private float _slowdownDuration = 1f;
    private float _slowdownFactor = 0.5f;

    public GameObject floatingTextPrefab;

    private bool _isInCampfire;
    private float _campfireDamageTimer;

    private void Awake()
    {
        floatingTextPrefab = Resources.Load<GameObject>("FloatingText");
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _healthBar = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;
        movementSpeed = Random.Range(minSpeed, maxSpeed);
        UpdateUI();
    }

    private void Update()
    {
        if (_isInCampfire)
        {
            _campfireDamageTimer += Time.deltaTime;
            if (_campfireDamageTimer >= 1f)
            {
                TakeDamage(15);
                _campfireDamageTimer = 0f;
            }
        }
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
            if (other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(10);
                _animator.SetTrigger("Attack");
            }
        }
        else if (other.gameObject.CompareTag("HoldableObject"))
        {
            CalculateDamageFromObject(other);
            SlowDownObject(other);

            if (other.gameObject.TryGetComponent<HoldableObject>(out var holdableObject) && holdableObject.isCampfire)
            {
                _isInCampfire = true;
                _campfireDamageTimer = 0f;
                holdableObject.AddEnemy(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HoldableObject"))
        {
            if (other.gameObject.TryGetComponent<HoldableObject>(out var holdableObject) && holdableObject.isCampfire)
            {
                _isInCampfire = false;
                holdableObject.RemoveEnemy(this);
            }
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
                    _animator.SetTrigger("Attack");
                }
            }
        }
    }

    private void SlowDownObject(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.velocity *= 0.7f;
        }
    }

    private void CalculateDamageFromObject(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<HoldableObject>(out var holdableObject) && other.attachedRigidbody != null)
        {
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