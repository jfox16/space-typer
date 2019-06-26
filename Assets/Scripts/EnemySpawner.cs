using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 1;

    void Start() {
        InvokeRepeating("Spawn", 0, spawnRate);
    }

    void Spawn() {
        GameObject _obj = GameController.Instance.enemyPooler.ActivateObject();
        if (_obj != null) {
            Enemy _enemy = _obj.GetComponent<Enemy>();
            _enemy.transform.position = transform.position;
            _enemy.Activate();
        }
    }
}
