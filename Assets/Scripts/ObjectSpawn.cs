using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn = null;
    [SerializeField] bool spawnOnStart = false;

    void Start() {
        if (spawnOnStart) {
            Invoke("Spawn", 0);
        }
    }

    public void Spawn() {
        PoolController.Activate(objectToSpawn, transform.position, transform.rotation);
    }
}
