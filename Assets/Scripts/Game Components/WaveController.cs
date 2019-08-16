using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public static WaveController Instance = null;

    WaveSpawner[] waveSpawners;
    int current_index = 0;
    float timeToFirstSpawn = 0;

    void Awake() {
        Instance = this;
        waveSpawners = GetComponentsInChildren<WaveSpawner>();
    }

    void Start()
    {
        InitializeWaveSpawners();
        if (waveSpawners.Length >= 1) {
            Invoke("StartNextWave", timeToFirstSpawn);
        }
    }

    // Wave Spawners spawn in order of their y-position.
    // This function determines the time between one WaveSpawner and the next,
    // based on the difference between their y-positions.
    void InitializeWaveSpawners()
    {
        WaveSpawner _lastWs = null;
        foreach (WaveSpawner _ws in waveSpawners) {
            if (_lastWs == null) {
                float _time = _ws.transform.position.y - transform.position.y;
                timeToFirstSpawn = _time;
                _lastWs = _ws;
            }
            else {
                float _time = _ws.transform.position.y - _lastWs.transform.position.y;
                _lastWs.timeToNextWave = _time;
                _lastWs = _ws;
            }
        }
    }

    void StartNextWave() {
        if (current_index < waveSpawners.Length) {
            WaveSpawner _waveSpawner = waveSpawners[current_index];
            _waveSpawner.SpawnObjects();
            current_index++;
            Invoke("StartNextWave", _waveSpawner.timeToNextWave);
        }
    }
}
