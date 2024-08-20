using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    public AudioClip deathSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip walkSound;

    private Transform _target;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private AudioSource _audioSource;

    public float minSpeed = 1f;
    public float maxSpeed = 1.5f;
    private float movementSpeed;

    public float maxHealth = 100f;
    public float currentHealth;

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
        _audioSource = GetComponent<AudioSource>();
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

        // Play walk sound when the enemy is moving
        if (_rb.velocity.magnitude > 0 && !_audioSource.isPlaying)
        {
            if (walkSound != null)
            {
                if (_audioSource != null)
                {
                    _audioSource.PlayOneShot(walkSound, 0.5f); // Adjust volume as needed
                }
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (deathSound != null)
            {
                SoundManager.Instance.PlaySFX(deathSound, 1f);
            }
            currentHealth = 0;
            Die();
        }
        else
        {
            _animator.SetTrigger("OnHit");
            StartCoroutine(SlowdownEffect());
            if (hitSound != null)
            {
                _audioSource.PlayOneShot(hitSound, 1f); // Play hit sound
            }
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
        _animator.Play("Die");  // Play the Die animation immediately
        StartCoroutine(DeathAnimation());  // Start a coroutine to wait for the animation to finish
    }

    private IEnumerator DeathAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);  // Wait for the current animation to finish
        Destroy(gameObject);  // Destroy the game object after the animation
    }


    private void UpdateUI()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(10);
                _animator.SetTrigger("Attack");
                if (attackSound != null)
                {
                    _audioSource.PlayOneShot(attackSound, 1f); // Play attack sound
                }
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