using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * MainCameraTransformer is a class to be attached to the Main Camera game object in a Scene.
 * It does the moving camera effects by changing velocity frame by frame, giving it a more 
 * realistic movement pattern. 
 */
public class MainCameraTransformer : MonoBehaviour
{
    // Input
    public static MainCameraTransformer Instance;
    public bool debugThis = false;

    // Positioning
    public Vector3 eulerRotation = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    Vector3 centerRotation = Vector3.zero;
    public float rotationSpeed = 15;
    public float rotationalDrag = 1;
    public float springBackForce = 0.1f;
    public float maxRotation = 10;

    const float minMagnitude = 0.1f;

    void Awake()
    {
        Instance = this;

        Camera camera = GetComponent<Camera>();
        if (camera == null) return;


    }

    void Update()
    {

        eulerRotation += velocity * rotationSpeed * Time.deltaTime;
        eulerRotation = Vector3.ClampMagnitude(eulerRotation, maxRotation);
        transform.rotation = Quaternion.Euler(eulerRotation);

        velocity = Vector3.MoveTowards(velocity, Vector3.zero, rotationalDrag * Time.deltaTime);
        Vector3 springBack = (centerRotation - eulerRotation) * springBackForce * Time.deltaTime;
        velocity += springBack;

        // if (eulerRotation.magnitude < minMagnitude) eulerRotation = Vector3.zero;
        // if (velocity.magnitude < minMagnitude) velocity = Vector3.zero;

        if (debugThis) {
            Debug.Log("eulerRotation: " + eulerRotation);
            Debug.Log("springBack: " + springBack);
            Debug.Log("velocity: " + velocity);
        }
    }

    public static void SetCenterRotation(Vector3 eulerRotation)
    {
        Instance.centerRotation = eulerRotation;
    }

    public static void AddVelocity(Vector3 eulerRotation)
    {
        Instance.velocity += eulerRotation;
    }
}
