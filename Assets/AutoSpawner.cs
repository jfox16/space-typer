using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawner : MonoBehaviour
{
    [SerializeField] int numberToSpawn = 5;
    [SerializeField] float timeBetweenSpawns = 5;
    [SerializeField] List<GameObject> commonSpawnObjects;
    [SerializeField] List<GameObject> rareSpawnObjects;

    BoxCollider2D spawnArea;
    Timer spawnTimer;


    void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        spawnTimer = new Timer();
    }

    void Update()
    {
        if (spawnTimer.isDone) {
            RandomSpawn();
            spawnTimer.SetTime(timeBetweenSpawns);
        }
    }

    void RandomSpawn()
    {
        for (int i=0; i<numberToSpawn; i++) 
        {
            // Choose random spawn object
            GameObject randomSpawnObject;
            randomSpawnObject = commonSpawnObjects[Random.Range(0, commonSpawnObjects.Count)];

            // int rarityCheck = Random.Range(0, 8);
            // if (rarityCheck == 0) {
            //     randomSpawnObject = rareSpawnObjects[Random.Range(0, rareSpawnObjects.Count)];
            // }
            // else {
            //     randomSpawnObject = commonSpawnObjects[Random.Range(0, commonSpawnObjects.Count)];
            // }



            // Choose random position
            float randomXPos = (Random.value - 0.5f) * spawnArea.size.x;
            float randomYPos = (Random.value - 0.5f) * spawnArea.size.y;
            Vector3 spawnPosition = transform.position + (transform.rotation * new Vector3(randomXPos, randomYPos));

            Global.Instantiate(
                randomSpawnObject, 
                spawnPosition,
                transform.rotation
            );
        }
    }
}
