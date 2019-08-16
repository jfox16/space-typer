using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The Shuriken projectile speeds up after being fired. This is done using the speedUpFactor variable. */
public class Shuriken : Projectile
{
    [SerializeField] float speedUpRate = 1.0f;
    float speedUpFactor = 1;

    protected override void Update()
    {
        base.Update();
        speedUpFactor += speedUpRate * Time.deltaTime;
    }

    public override void Spawn() 
    {
        base.Spawn();
        speedUpFactor = 1;
    }

    protected override void Move()
    {
        float spedUpMoveSpeed = moveSpeed * speedUpFactor;
        Vector3 moveVector = forwardVector * spedUpMoveSpeed * Time.deltaTime;
        transform.position += moveVector;

        if (CheckMovingOutOfBounds()) {
            Deactivate();
        }
    }
}
