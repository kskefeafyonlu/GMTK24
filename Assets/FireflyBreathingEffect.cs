using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class FireflyGlow : MonoBehaviour
{
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 1f; // Maximum light intensity
    public float pulseSpeed = 1f; // Speed of the pulse
    public SpriteRenderer sprite; // Manually assign this in the inspector

    private Light2D _light;

    void Start()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        // Calculate a new intensity based on a sine wave over time
        float newIntensity = minIntensity + Mathf.Sin(Time.time * pulseSpeed) * (maxIntensity - minIntensity) / 2f;

        // Apply the new intensity to the light
        _light.intensity = newIntensity;

        // Sync the sprite's alpha with the light's intensity
        Color spriteColor = sprite.color;
        spriteColor.a = newIntensity;
        sprite.color = spriteColor;
    }
}
