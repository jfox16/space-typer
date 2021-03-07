using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingGameField : MonoBehaviour
{
    public static TypingGameField Instance;

    List<List<TypingEnemy>> enemyLanes = new List<List<TypingEnemy>>();
    public float[] laneXs { get; private set; }

    [SerializeField] GameObject laneMarkerPrefab = null;

    int numLanes { get { return TypingGameController.NUM_LANES; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        laneXs = new float[numLanes];

        for (int i = 0; i < numLanes; i++)
        {
            float x = TypingGameController.LANE_DISTANCE * (i - numLanes/2);
            laneXs[i] = x;
            List<TypingEnemy> enemyLane = new List<TypingEnemy>();
            enemyLanes.Add(enemyLane);
            Instantiate(laneMarkerPrefab, new Vector3(x, TypingGameController.PLAYER_Y, 0), Quaternion.identity);
        }
    }

    public TypingEnemy SpawnEnemy(GameObject enemyPrefab, int lane, string word)
    {
        Vector3 enemyPos = new Vector3( laneXs[lane], TypingGameController.ENEMY_SPAWN_Y, 0 );

        GameObject enemyGo = PoolController.Activate( enemyPrefab, enemyPos, Quaternion.identity );

        if (enemyGo == null)
        {
            return null;
        }

        TypingEnemy enemy = enemyGo.GetComponent<TypingEnemy>();
        enemy.SetTargetWord(word);
        enemy.lane = lane;

        enemyLanes[lane].Add(enemy);

        return enemy;
    }

    public void RemoveEnemy(TypingEnemy enemy)
    {
        List<TypingEnemy> enemies = enemyLanes[enemy.lane];
        bool removed = enemies.Remove(enemy);
    }

    // Find the closest enemy in current lane that matches the character pressed
    public TypingEnemy FindTarget(int lane, char letter)
    {
        List<TypingEnemy> enemies = enemyLanes[lane];

        TypingEnemy target = null;

        for(int i = 0; i < enemies.Count; i++)
        {
            TypingEnemy enemy = enemies[i];

            if (enemy == null || !enemy.isAlive)
            {
                continue;
            }

            if (enemy.firstLetter == letter)
            {
                if (!target || enemy.transform.position.y < target.transform.position.y)
                {
                    target = enemy;
                }
            }
        }

        return target;
    }
}
