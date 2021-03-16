using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingGameController : MonoBehaviour
{
    public static TypingGameController Instance;

    public const int NUM_LANES = 5;
    public const int NUM_WORDS = 100;
    public const float LANE_DISTANCE = 5.0f;
    public const float PLAYER_Y = -6.5f;
    public const float ENEMY_SPAWN_Y = 8.5f;
    public const float ENEMY_DIE_Y = -5.0f;
    public const float PLAYER_MOVE_TIME = 0.1f;

    Timer enemySpawnTimer;
    [SerializeField] float enemySpawnTime = 4.0f;

    public int shieldHealth = 30;

    [SerializeField] GameObject enemyPrefab = null;
    [SerializeField] GameObject enemyPrefabHard = null;
    [SerializeField] GameObject enemyPrefabBaby = null;

    public AudioClip typeSound = null;

    // "Player" variables
    [SerializeField] TypingGamePlayer player = null;
    TypingEnemy currentTarget = null;
    int currentIndex = 0;
    int currentLane = 2;
    bool isMoving = false;
    int queuedLaneMoves = 0;
    char queuedLetter = '\0';

    List<TypingEnemy> currentWaveEnemies = new List<TypingEnemy>();
    bool waveStarted = false;

    void Awake()
    {
        Instance = this;
        enemySpawnTimer = new Timer(2);
    }

    void Start()
    {
    }

    void Update()
    {
        // Move player
        if (!isMoving)
        {
            if (queuedLaneMoves > 0)
            {
                queuedLaneMoves--;
                MoveLaneDirection(1);
            }
            else if (queuedLaneMoves < 0)
            {
                queuedLaneMoves++;
                MoveLaneDirection(-1);
            }

            if (queuedLetter != '\0')
            {
                PressKey(queuedLetter);
                queuedLetter = '\0';
            }
        }

        if (enemySpawnTimer.isDone)
        {
            SpawnWave();
            enemySpawnTimer.SetTime(enemySpawnTime);
        }
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            HandleKeyDown(e);
        }
    }

    void HandleKeyDown(Event e)
    {
        char letter = e.character;
        int ascii = (int)letter;

        if (ascii >= 65 && ascii <= 90)
        {
            // Turn capital letters lowercase
            PressKey((char)(ascii + 32));
        }
        else if (ascii >= 97 && ascii <= 122 || ascii == 32 || ascii == 45)
        {
            // lowercase letters, dash, space
            PressKey(letter);
        }
        else if (ascii >= 49 && ascii <= 53)
        {
            // Treat numbers as a move command
            MoveToLane(ascii - 49);
        }
        else
        {
            bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            
            switch(e.keyCode)
            {
                case KeyCode.Tab:
                    MoveLaneDirection(shiftHeld ? 1 : -1);
                    break;

                case KeyCode.Return:
                    MoveLaneDirection(shiftHeld ? -1 : 1);
                    break;

                case KeyCode.LeftArrow:
                    MoveLaneDirection(-1);
                    break;

                case KeyCode.RightArrow:
                    MoveLaneDirection(1);
                    break;
            }
        }
    }

    void PressKey(char letter)
    {
        if (isMoving)
        {
            queuedLetter = letter;
            return;
        }

        if (!currentTarget)
        {
            currentTarget = TypingGameField.Instance.FindTarget(currentLane, letter);
            if (!currentTarget) return;
        }

        string currentWord = currentTarget.targetWord;

        if (letter == currentWord[currentIndex])
        {
            currentIndex++;
            currentTarget.UpdateIndex(currentIndex);

            if (currentIndex < currentWord.Length)
            {
                player.Shoot(currentTarget);
            }
            else
            {
                player.ShootKillShot(currentTarget);
                TypingGameField.Instance.RemoveEnemy(currentTarget);
                ResetTarget();
            }
        }
    }

    void ResetTarget()
    {
        currentTarget = null;
        currentIndex = 0;
    }

    void MoveLaneDirection(int direction)
    {
        if (!isMoving)
        {
            int newLane = (currentLane + direction + NUM_LANES) % NUM_LANES;
            MoveToLane(newLane);
        }
        else
        {
            // queue a lane move
            queuedLaneMoves = direction;
            queuedLaneMoves = Mathf.Clamp(direction, -3, 3);
        }
    }

    void MoveToLane(int newLane)
    {
        if (isMoving) return;

        if (currentTarget != null)
        {
            currentTarget.UpdateIndex(0);
        }

        isMoving = true;
        ResetTarget();

        if ( currentLane == NUM_LANES-1 && newLane == 0 )
        {
            StartCoroutine(LoopAfterTime(newLane, 1));
        }
        else if ( currentLane == 0 && newLane == NUM_LANES-1 )
        {
            StartCoroutine(LoopAfterTime(newLane, -1));
        }
        else
        {
            StartCoroutine(MoveAfterTime(newLane));
        }
    }

    IEnumerator MoveAfterTime(int newLane)
    {
        float seconds = (Mathf.Abs(currentLane - newLane) + 1) * PLAYER_MOVE_TIME;
        float newLaneX = TypingGameField.Instance.laneXs[newLane];
        Vector3 newLanePosition = new Vector3( newLaneX, PLAYER_Y, player.transform.position.z );
        player.MoveToPosition(newLanePosition, seconds);

        yield return new WaitForSeconds(seconds);

        isMoving = false;
        currentLane = newLane;
        player.StopMoving();
        player.transform.position = newLanePosition;
    }

    // Same as MoveAfterTime, but for going off the screen and coming back on the other side
    IEnumerator LoopAfterTime(int newLane, int direction)
    {
        float seconds = PLAYER_MOVE_TIME;

        float currentLaneX = TypingGameField.Instance.laneXs[currentLane];
        float newLaneX = TypingGameField.Instance.laneXs[newLane];

        Vector3 firstPosition = new Vector3(
            currentLaneX + LANE_DISTANCE*direction,
            PLAYER_Y,
            player.transform.position.z
        );

        player.MoveToPosition(firstPosition, seconds);

        yield return new WaitForSeconds(seconds);

        player.transform.position = new Vector3(
            newLaneX - LANE_DISTANCE*direction,
            PLAYER_Y,
            player.transform.position.z
        );

        Vector3 secondPosition = new Vector3(
            newLaneX,
            PLAYER_Y,
            player.transform.position.z
        );

        player.MoveToPosition(secondPosition, seconds);

        yield return new WaitForSeconds(seconds);

        isMoving = false;
        currentLane = newLane;
        player.StopMoving();
        player.transform.position = secondPosition;
    }

    void SpawnWave()
    {
        float enemyDifficulty = Random.Range(0.0f, 1.0f);
        WordImporter.Difficulty wordDifficulty;
        GameObject spawnEnemyPrefab;
        int numberOfEnemies;

        if (enemyDifficulty < 0.4f)
        {
            wordDifficulty = WordImporter.Difficulty.Medium;
            spawnEnemyPrefab = enemyPrefab;
            numberOfEnemies = 3;
        }
        else if (enemyDifficulty < 0.7f)
        {
            wordDifficulty = WordImporter.Difficulty.Legendary;
            spawnEnemyPrefab = enemyPrefabHard;
            numberOfEnemies = 2;
        }
        else
        {
            wordDifficulty = WordImporter.Difficulty.Baby;
            spawnEnemyPrefab = enemyPrefabBaby;
            numberOfEnemies = 4;
        }

        int[] randomLanes = RandomUtil.RandomInts(0, NUM_LANES, numberOfEnemies);
        foreach (int lane in randomLanes)
        {
            string randomWord = WordImporter.GetWord(wordDifficulty);
            TypingEnemy enemy = TypingGameField.Instance.SpawnEnemy(spawnEnemyPrefab, lane, randomWord);
            currentWaveEnemies.Add(enemy);
        }
    }

    public void RemoveEnemy(TypingEnemy enemy)
    {
        if (currentTarget == enemy)
        {
            ResetTarget();
        }
        TypingGameField.Instance.RemoveEnemy(enemy);
        currentWaveEnemies.Remove(enemy);
    }
}
