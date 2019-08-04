using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] float timeBeforeFirstSpawn = 5.0f;

    WaveSpawner[] waveSpawners;
    int waveSpawner_i = 0;

    void Awake() {
        waveSpawners = GetComponentsInChildren<WaveSpawner>();
    }

    void Start()
    {
        Invoke("StartNextWave", timeBeforeFirstSpawn);
    }

    void StartNextWave() {
        if (waveSpawner_i < waveSpawners.Length) {
            WaveSpawner _waveSpawner = waveSpawners[waveSpawner_i];
            _waveSpawner.SpawnObjects();
            waveSpawner_i++;
            Invoke("StartNextWave", _waveSpawner.timeToNextWave);
        }
    }
}
