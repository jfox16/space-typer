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

    [SerializeField] GameObject energySpark = null;

    void Awake() {
        Instance = this; 
    }

    void Update() {
        // Press Esc to restart
        if (Input.GetButtonDown("Cancel")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public static void Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        PoolController.Activate(prefab, position, rotation);
    }
}
