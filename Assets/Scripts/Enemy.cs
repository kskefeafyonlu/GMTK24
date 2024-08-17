using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Transform _target;
    private Rigidbody2D _rb;

    public float Speed = 5f;
    
    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        PathFind();
    }

    private void PathFind()
    {

        Vector2 direction = (Vector2)_target.position - _rb.position;
        direction.Normalize();
        
        _rb.velocity = direction * Speed;
    }
}
