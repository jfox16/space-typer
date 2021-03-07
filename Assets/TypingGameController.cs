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
    public const float PLAYER_MOVE_TIME = 0.2f;

    [SerializeField] WordImporter.Difficulty difficulty = WordImporter.Difficulty.Easy;

    Timer enemySpawnTimer;
    public float enemySpawnTime = 4.0f;
    int enemySpawn_i;

    public int shieldHealth = 30;

    public GameObject enemyPrefab = null;

    public AudioClip typeSound = null;

    // "Player" variables
    public TypingGamePlayer player = null;
    TypingEnemy currentTarget = null;
    int currentIndex = 0;
    int currentLane = 2;
    bool isMoving = false;
    int queuedLaneMoves = 0;
    char queuedLetter = (char) 0;

    //debug
    string currentWord;

    int lastRandomLane = -1;

    string[] words;

    void Awake()
    {
        Instance = this;
        words = WordImporter.GetWords(difficulty, NUM_WORDS);
        enemySpawnTimer = new Timer(5);
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

            if (queuedLetter != (char)0)
            {
                PressKey(queuedLetter);
                queuedLetter = (char)0;
            }
        }

        // Spawn enemies
        if (enemySpawnTimer.isDone)
        {
            string randomWord = words[Random.Range(0, words.Length)];
            int randomLane = (int) Random.Range(0, NUM_LANES-1);
            if (randomLane == lastRandomLane) {
                randomLane = (int) Random.Range(0, NUM_LANES-1);
            }
            lastRandomLane = randomLane;
            
            TypingGameField.Instance.SpawnEnemy(enemyPrefab, randomLane, randomWord);
            enemySpawnTimer.SetTime(enemySpawnTime);
            enemySpawn_i++;
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

        if (ascii >= 97 && ascii <= 122 || ascii == 32 || ascii == 45)
        {
            PressKey(letter);
        }
        else
        {
            switch(e.keyCode)
            {
                case KeyCode.Tab:
                    bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    int direction = (shiftHeld ? -1 : 1);
                    MoveLaneDirection(direction);
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

        string word = currentTarget.targetWord;
        currentWord = word;

        if (letter == word[currentIndex])
        {
            currentIndex++;
            currentTarget.UpdateIndex(currentIndex);

            player.PlayKeyPress();

            if (currentIndex < word.Length)
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
        currentWord = "";
    }

    void MoveLaneDirection(int direction)
    {
        if (!isMoving)
        {
            int newLane = (currentLane + direction + NUM_LANES) % NUM_LANES;
            if (currentTarget != null)
            {
                currentTarget.UpdateIndex(0);
            }
            MoveToLane(newLane);
        }
        else
        {
            // queue a lane move
            queuedLaneMoves = direction;
            queuedLaneMoves = Mathf.Clamp(direction, -5, 5);
        }
    }

    void MoveToLane(int newLane)
    {
        if (isMoving) return;

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
        float seconds = Mathf.Abs(currentLane - newLane) * PLAYER_MOVE_TIME;
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
        float seconds = PLAYER_MOVE_TIME / 2;

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

    void SpawnEnemy()
    {
        // PoolController.Activate(enemyPrefab, )
    }

    public void RemoveEnemy(TypingEnemy enemy)
    {
        if (currentTarget == enemy)
        {
            ResetTarget();
        }
        TypingGameField.Instance.RemoveEnemy(enemy);
    }
}
