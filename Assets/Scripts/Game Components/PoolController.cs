using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**

Spawn and despawn GameObjects through this class.

- To spawn a GameObject:
    PoolController.Instance.Activate(gameObjectPrefab, position, rotation);

- To despawn a GameObject:
    PoolController.Instance.Deactivate(gameObject);

 */

public class PoolController : MonoBehaviour
{
    public static PoolController Instance;

    [SerializeField] bool showDebug = false;
    Dictionary<string, ObjectPooler> poolDict = new Dictionary<string, ObjectPooler>();

    void Awake() 
    {
        Instance = this;

        ObjectPooler[] _objectPoolers = GetComponentsInChildren<ObjectPooler>();
        Debug.Log("[PoolController] Number of object poolers: " + _objectPoolers.Length);
        foreach(ObjectPooler _pooler in _objectPoolers) {
            string _objectName = _pooler.objectToPool.name;
            poolDict.Add(_objectName, _pooler);
            if (showDebug) {
                Debug.Log("[PoolController] Added " + _objectName + " pooler");
            }
        }
    }

    /// <summary>
    /// Activates a GameObject from the appropriate pool, if available.
    /// Returns a reference to the GameObject if successful, null otherwise.
    /// </summary>
    public static GameObject Activate(GameObject objectToActivate, Vector3 position, Quaternion rotation) 
    {
        // Find the Object Pooler that matches desired object
        string _objectName = objectToActivate.name;
        ObjectPooler _pooler;
        Instance.poolDict.TryGetValue(_objectName, out _pooler);
        // Activate an object from the pooler and return it
        if (_pooler != null) {
            GameObject _activatedObject = _pooler.ActivateObject();
            if (_activatedObject != null) {
                _activatedObject.transform.SetPositionAndRotation(position, rotation);
                return _activatedObject;
            }
            else {
                Debug.LogError("[PoolController] Activate() failed.");
                return null;
            }
        }
        // Return null if no pool available
        else {
            Debug.LogError("No pool available for " + _objectName);
            return null;
        }
    }

    /// <summary>
    /// Deactivates a GameObject and returns it to the appropriate pool, if available.
    /// </summary>
    public static void Deactivate(GameObject objectToDeactivate) 
    {
        // Find the Object Pooler that matches desired object
        string _objectName = objectToDeactivate.name;
        _objectName = _objectName.Replace("(Clone)", "");
        ObjectPooler _pooler;
        Instance.poolDict.TryGetValue(_objectName, out _pooler);

        if (_pooler != null) {
            _pooler.DeactivateObject(objectToDeactivate);
        }
        else {
            Debug.LogError("No pool available for " + _objectName);
        }
    }
}
