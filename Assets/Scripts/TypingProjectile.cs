using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TypingProjectile : Poolable
{
    TypingEnemy target;
    public float speed = 1.0f;
    public new Collider2D collider = null;
    bool isKillShot = false;
    [SerializeField] GameObject hitSpark = null;
    [SerializeField] float knockbackForce = 3.0f;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!target.isAlive)
        {
            Die();
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        if (transform.position == target.transform.position)
        {
            if (!isKillShot)
            {
                target.Hurt(knockbackForce);
            }
            else
            {
                target.Kill(knockbackForce);
            }
            Die();
        }
    }

    public override void Die()
    {
        Quaternion random2DRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
        PoolController.Activate( hitSpark, transform.position, random2DRotation );

        base.Die();
    }

    public void Initialize(TypingEnemy target, bool isKillShot)
    {
        this.target = target;
        this.isKillShot = isKillShot;
    }
}