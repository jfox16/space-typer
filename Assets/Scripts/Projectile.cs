using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Unit
{
    public float moveSpeed = 1.0f;
    public float damage = 5.0f;

    void Update() 
    {
        transform.position += transform.up * moveSpeed * 0.1f;
        CheckBounds();
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        Unit _unit = col.GetComponent<Unit>();
        if (_unit != null && _unit.isAlive && (_unit.team == Unit.Team.Neutral || _unit.team != team) ) {
            _unit.Hurt(damage);
            Die();
        }
    }

    void CheckBounds() 
    {
        // Destroy Projectile if it goes out of bounds
        if (transform.position.x < GameController.Instance.xBoundMin ||
            transform.position.x > GameController.Instance.xBoundMax ||
            transform.position.y < GameController.Instance.yBoundMin ||
            transform.position.y > GameController.Instance.yBoundMax) 
        {
            Die();
        }
    }

    void Die() 
    {
        PoolController.Deactivate(gameObject);
    }
}
