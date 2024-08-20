// FloatingText.cs
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }

    public void Initialize(float damage)
    {
        _textMesh.text = Mathf.RoundToInt(damage).ToString();
        StartCoroutine(FadeAndFloatText());
    }

    private IEnumerator FadeAndFloatText()
    {
        Color originalColor = _textMesh.color;
        Vector3 originalPosition = transform.position;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            _textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            transform.position = originalPosition + new Vector3(0, elapsedTime, 0);
            yield return null;
        }

        Destroy(gameObject);
    }
}