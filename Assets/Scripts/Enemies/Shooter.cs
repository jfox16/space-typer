using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    [SerializeField] GameObject projectile = null;
    [SerializeField] float timeToFirstFire = 0.0f;
    [SerializeField] float timeBetweenFires = 2.0f;
    [SerializeField] GameObject fireLocation = null;
    Timer fireTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        fireTimer = new Timer(timeToFirstFire);
    }

    protected override void Update()
    {
        base.Update();
        if (isAlive && fireTimer.isDone) {
            Fire();
            fireTimer.SetTime(timeBetweenFires);
        }
    }

    void Fire() 
    {
        Vector3 _projPos = fireLocation.transform.position;
        Quaternion _projRot = fireLocation.transform.rotation;
        GameObject _projGo = PoolController.Activate(projectile, _projPos, _projRot);
        Projectile _proj = _projGo.GetComponent<Projectile>();
    }

    public override void Hurt(float damage) 
    {
        base.Hurt(damage);
        fireTimer.SetTime(timeBetweenFires);
    }
}
