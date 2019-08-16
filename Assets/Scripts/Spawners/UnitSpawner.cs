using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

This script spawns Units.
If spawnOnStart is set to true, object will be spawned at the start of the game.

 */
public class UnitSpawner : MonoBehaviour
{
    public GameObject objectToSpawn = null;
    [SerializeField] float defaultSpawnRotationInDegrees = 0;
    [SerializeField] bool spawnOnStart = false;

    void Start() {
        if (spawnOnStart) {
            Invoke("Spawn", 0);
        }
    }

    public virtual GameObject Spawn() {
        Quaternion defaultSpawnRotation = Quaternion.Euler(0, 0, defaultSpawnRotationInDegrees);
        return Spawn(transform.rotation*defaultSpawnRotation);
    }
    public virtual GameObject Spawn(Quaternion spawnRotation) {
        Unit _unit = objectToSpawn.GetComponent<Unit>();
        if (_unit != null) {
            return PoolController.Activate(objectToSpawn, transform.position, spawnRotation);
        }
        else {
            Debug.LogError("[UnitSpawner] objectToSpawn is not a Unit!");
            return null;
        }
    }


    

    // CheckOutOfBounds() returns true if this Unit is out of bounds, false otherwise.
    // Will return false if Unit is within extraMargin meters of bounds.
    protected bool CheckOutOfBounds()
    {
        return CheckOutOfBounds(0);
    }
    protected bool CheckOutOfBounds(float extraMargin)
    {
        if (transform.position.y + extraMargin < Global.yBoundMin)
            return true;
        else if (transform.position.y - extraMargin > Global.yBoundMax)
            return true;
        else if (transform.position.x + extraMargin < Global.xBoundMin)
            return true;
        else if (transform.position.x - extraMargin > Global.xBoundMax)
            return true;
        else
            return false;
    }

    // CheckMovingOutOfBounds() returns true if this Unit is moving out of bounds, false otherwise.
    // Returns false if Unit is moving inward from out of bounds.
    // Returns false if Unit is within extraMargin meters of bounds.
    protected bool CheckMovingOutOfBounds()
    {
        return CheckMovingOutOfBounds(0);
    }
    protected bool CheckMovingOutOfBounds(float extraMargin)
    {
        float angleOfRotation = transform.rotation.eulerAngles.z;

        // Going down or up
        if (angleOfRotation > 180.0f && angleOfRotation < 360.0f) {
            if (transform.position.y + extraMargin < Global.yBoundMin)
                return true;
        }
        else {
            if (transform.position.y - extraMargin > Global.yBoundMax)
                return true;
        }

        // Going left or right
        if (angleOfRotation > 90.0f && angleOfRotation < 270.0f) {
            if (transform.position.x + extraMargin < Global.xBoundMin)
                return true;
        }
        else {
            if (transform.position.x - extraMargin > Global.xBoundMax)
                return true;
        }

        return false;
    }

}
