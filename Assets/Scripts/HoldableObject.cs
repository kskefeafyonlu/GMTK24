using System.Collections.Generic;
using UnityEngine;

public class HoldableObject : MonoBehaviour
{
    public AudioClip breakingSound;

    public float Mass = 1f;
    public float MaxScale = 2f;
    public float MinScale = 0.5f;
    public int HP = 2;
    public bool isCampfire;

    private List<Enemy> _enemiesInCollision = new List<Enemy>();
    private float _damageTimer;

    public int CalculateDamage()
    {
        if (transform.localScale.x >= MaxScale)
        {
            return 50;
        }
        else if (transform.localScale.x <= MinScale)
        {
            return 10;
        }

        return 0;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            SoundManager.Instance.PlaySFX(breakingSound, 1f);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_enemiesInCollision.Count > 0)
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= 1f)
            {
                TakeDamage(_enemiesInCollision.Count); // Decrease HP by the number of enemies every second
                _damageTimer = 0f;
            }
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        if (!_enemiesInCollision.Contains(enemy))
        {
            _enemiesInCollision.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (_enemiesInCollision.Contains(enemy))
        {
            _enemiesInCollision.Remove(enemy);
        }
    }
}