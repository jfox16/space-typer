using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    public static Global Instance;

    public static float xBoundMin = -12.5f;
    public static float xBoundMax = 12.5f;
    public static float yBoundMin = -7.5f;
    public static float yBoundMax = 10.0f;

    public enum GameState {Off, Running, Paused}
    public static GameState gameState = GameState.Off;

    //Controls
    bool ctrlPressed, altPressed, escPressed, resetted;

    void Awake() {
        Instance = this; 
    }

    void Update() 
    {
        bool escPressed = Input.GetKey(KeyCode.Escape);

        // Press Esc to pause
        if (escPressed) {
            gameState = GameState.Paused;
        }

        // Pressing Ctrl + Alt + Esc at the same time will restart the game.
        // 
        if (ctrlPressed && altPressed && escPressed) 
        {
            if (resetted == false) {
                resetted = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else {
            resetted = false;
        }
    }

    public static void Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        PoolController.Activate(prefab, position, rotation);
    }
}
