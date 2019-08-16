using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : UnitSpawner
{
    public enum AimType {Directional, TargetPlayer}
    public AimType aimType;

    [SerializeField] float timeToFirstShot = 0.0f;
    [SerializeField] float timeBetweenShots = 2.0f;

    Unit unitAttachedTo = null;

    bool hasStartedFiring = false;
    Timer shootTimer;



    protected virtual void Awake()
    {
        shootTimer = new Timer();
        unitAttachedTo = transform.parent.GetComponent<Unit>();
    }

    protected virtual void Update()
    {
        // Don't start firing until Shooter is within game boundaries.
        if (hasStartedFiring == false) {
            if (!CheckOutOfBounds()) {
                hasStartedFiring = true;
                shootTimer.SetTime(timeToFirstShot);
            }
        }
        else {
            if (unitAttachedTo.isAlive && shootTimer.isDone) {
                Spawn();
                shootTimer.SetTime(timeBetweenShots);
            }
            if (CheckOutOfBounds()) {
                hasStartedFiring = false;
            }
        }
    }

    public override GameObject Spawn()
    {
        GameObject spawnedObject;

        switch(aimType)
        {
            case AimType.TargetPlayer:
                spawnedObject = ShootAtPlayer();
                break;
            default:
                spawnedObject = ShootForward();
                break;
        }

        return spawnedObject;
    }

    GameObject ShootForward()
    {
        GameObject spawnedObject = PoolController.Activate(objectToSpawn, transform.position, transform.rotation);
        return spawnedObject;
    }

    GameObject ShootAtPlayer()
    {
        GameObject spawnedObject = PoolController.Activate(objectToSpawn, transform.position, transform.rotation);
        Unit spawnedUnit = spawnedObject.GetComponent<Unit>(); 
        spawnedUnit.TurnThisTowards(Player.Instance.transform.position);
        return spawnedObject;
    }
}
