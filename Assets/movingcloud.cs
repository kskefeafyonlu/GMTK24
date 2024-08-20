using UnityEngine;
using UnityEngine.UI;

public class CloudMovement : MonoBehaviour
{
    public float speed = 100.0f; // Adjust this value to change the speed of the cloud
    private RectTransform rectTransform;
    private float widthOfImage;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        widthOfImage = rectTransform.rect.width; // Get the width of the image
    }

    void Update()
    {
        // Move the cloud to the right over time
        rectTransform.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);

        // If the cloud has moved off the right side of the screen, move it back to the left
        if (rectTransform.anchoredPosition.x > Screen.width)
        {
            rectTransform.anchoredPosition = new Vector2(-widthOfImage, rectTransform.anchoredPosition.y);
        }
    }
}

