using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public float timeToNextWave = 5.0f;

    ObjectSpawn[] objectSpawns;

    void Awake() {
        objectSpawns = GetComponentsInChildren<ObjectSpawn>();
    }

    public virtual void SpawnObjects() {
        foreach (ObjectSpawn _objectSpawn in objectSpawns) {
            _objectSpawn.Spawn();
        }
    }
}
