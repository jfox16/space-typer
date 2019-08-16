using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    // timeToNextWave will be determined based on y-position.
    [HideInInspector] public float timeToNextWave = 0;

    UnitSpawner[] objectSpawns; 

    void Awake() {
        objectSpawns = GetComponentsInChildren<UnitSpawner>();
    }

    public virtual void SpawnObjects() {
        // Move to position before spawning
        transform.position = new Vector3(transform.position.x, WaveController.Instance.transform.position.y, 0);
        foreach (UnitSpawner _objectSpawn in objectSpawns) {
            _objectSpawn.Spawn();
        }
    }
}
