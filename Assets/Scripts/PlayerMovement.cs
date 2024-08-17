using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public GameObject playerGun;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    public bool isFacingRight = true; // Track the character's facing direction

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        ProcessInputs();
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
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
        else if (_moveDirection.x > 0 && !isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
    }
}