using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    private Transform _target;
    private Rigidbody2D _rb;

    public float MovementSpeed = 1f;
    
    public float MaxHealth = 100f;
    public float CurrentHealth;
    private Slider _healthBar;
    
    
    
    
    
    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        
        
        _healthBar = GetComponentInChildren<Slider>();
        CurrentHealth = MaxHealth;
        UpdateUI();
    }

    private void FixedUpdate()
    {
        PathFind();
    }

    private void PathFind()
    {

        Vector2 direction = (Vector2)_target.position - _rb.position;
        direction.Normalize();
        
        _rb.velocity = direction * MovementSpeed;
    }
    
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            
            Die();
        }
        UpdateUI();
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
    
    private void UpdateUI()
    {
        _healthBar.value = CurrentHealth / MaxHealth;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit by player");
            if (other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(10);
            }
        }
        else if (other.gameObject.CompareTag("HoldableObject"))
        {
            Debug.Log("Hit by holdable object");
            CalculateDamageFromObject(other);
        }
    }

    private void CalculateDamageFromObject(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<HoldableObject>(out var holdableObject) && other.rigidbody != null)
        {
            Debug.Log("Calculating damage");
            float damage = CalculateDamage(holdableObject.Mass, other.rigidbody.velocity.magnitude);
            TakeDamage(damage);
        }
    }

    private float CalculateDamage(float mass, float velocity)
    {
        return mass * velocity * velocity * 0.5f;
    }
}
