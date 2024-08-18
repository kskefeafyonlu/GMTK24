using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int _maxHealth = 100;
    private int _health = 100;
    private float currentHealth;

    [SerializeField] private Slider healthSlider;
    private TextMeshProUGUI healthText;

    private float lerpSpeed = 5f;

    private void Awake()
    {
        healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        currentHealth = _health;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        // Smoothly interpolate the health slider value
        healthSlider.value = Mathf.Lerp(healthSlider.value, (float)_health / _maxHealth, Time.deltaTime * lerpSpeed);
        UpdateUI();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
        UpdateUI();
    }

    public void Heal(int amount)
    {
        _health += amount;
        if (_health > 100)
        {
            _health = 100;
        }
        UpdateUI();
    }

    public void Die()
    {
        Debug.Log("Player died");
    }

    private void UpdateUI()
    {
        // Smoothly interpolate the health text value
        currentHealth = Mathf.Lerp(currentHealth, _health, Time.deltaTime * lerpSpeed);
        healthText.text = Mathf.RoundToInt(currentHealth).ToString();
    }
}