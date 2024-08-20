using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{

    
    public Animator animator;
    public float moveSpeed;
    public GameObject playerGun;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    public bool isFacingRight = true; // Track the character's facing direction
    private SpriteRenderer _spriteRenderer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ProcessInputs();
        animator.SetFloat(SpeedHash, _moveDirection.magnitude);
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        _rb.velocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);

        // Flip the character's axis based on the movement direction
        if (_moveDirection.x < 0 && isFacingRight)
        {
            _spriteRenderer.flipX = false;
            isFacingRight = false;
        }
        else if (_moveDirection.x > 0 && !isFacingRight)
        {
            _spriteRenderer.flipX = true;
            isFacingRight = true;
        }
    }
}