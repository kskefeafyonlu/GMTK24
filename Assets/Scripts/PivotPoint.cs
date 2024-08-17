using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    private Camera _mainCam;
    public float rotationSpeed = 5f;
    private PlayerMovement _playerMovement;
    private void Awake()
    {
        _mainCam = Camera.main;
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }
    private void FixedUpdate()
    {
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        Vector2 mouseWorldPosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        float targetAngle = Mathf.Atan2(mouseWorldPosition.y - transform.position.y, mouseWorldPosition.x - transform.position.x) * Mathf.Rad2Deg;

        if (!_playerMovement.isFacingRight)
        {
            targetAngle += 180; // Adjust the angle if the character is facing left
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}