// PlayerHealth.cs
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

    private bool _isInvincible = false;
    private float _invincibilityDuration = 0.2f;
    private float _invincibilityTimer = 0f;

    private void Awake()
    {
        healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        currentHealth = _health;
        UpdateUI();
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

        // Update invincibility timer
        if (_isInvincible)
        {
            _invincibilityTimer -= Time.deltaTime;
            if (_invincibilityTimer <= 0f)
            {
                _isInvincible = false;
            }
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
        UpdateUI();
        StartInvincibility();
    }

    public void Heal(int amount)
    {
        _health += amount;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
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
        healthSlider.value = (float)_health / _maxHealth;
    }

    private void StartInvincibility()
    {
        _isInvincible = true;
        _invincibilityTimer = _invincibilityDuration;
    }
}