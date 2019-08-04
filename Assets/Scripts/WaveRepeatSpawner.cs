using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRepeatSpawner : WaveSpawner
{
    [SerializeField] float timeBetweenSpawns = 10.0f;

    public override void SpawnObjects() {
        base.SpawnObjects();
        Invoke("SpawnObjects", timeBetweenSpawns);
    }
}
