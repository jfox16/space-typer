using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NPCUnit
{
    [SerializeField] GameObject hitSpark = null;

    protected override void Awake() 
    {
        base.Awake();

        collider = GetComponent<Collider2D>();
    }

    public override int DealDamageTo(Unit targetUnit)
    {
        int damageDealt = base.DealDamageTo(targetUnit);

        if (damageDealt > 0) {
            if (hitSpark != null) {
                Vector3 sparkLocation = collider.bounds.ClosestPoint(targetUnit.transform.position);
                Global.Instantiate(hitSpark, sparkLocation, transform.rotation);
            }
        }
        return damageDealt;
    }
}
