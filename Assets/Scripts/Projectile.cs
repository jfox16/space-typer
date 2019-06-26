using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum Type {Laser, Shuriken}
    public Type type;
    public Unit.Team team;
    public float moveSpeed = 1.0f;
    public float damage = 5.0f;
    public Vector2 moveDirection = Vector3.up;

    void Update() {
        transform.position += Vector3.up * moveSpeed * 0.1f;

        if (transform.position.x < GameController.Instance.xBoundMin ||
            transform.position.x > GameController.Instance.xBoundMax ||
            transform.position.y < GameController.Instance.yBoundMin ||
            transform.position.y > GameController.Instance.yBoundMax) 
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        Unit _unit = col.GetComponent<Unit>();
        if (_unit != null && _unit.isAlive && (_unit.team == Unit.Team.Neutral || _unit.team != team) ) {
            _unit.Hurt(damage);
            Die();
        }
    }

    void Die() {
        GameController.DeactivateProjectile(gameObject, type);
    }
}
