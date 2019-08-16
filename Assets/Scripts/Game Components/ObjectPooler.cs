using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject objectToPool = null;
    [SerializeField] int numberToPool = 1;

    Dictionary<int, GameObject> inactiveObjects = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> activeObjects = new Dictionary<int, GameObject>();

    void Awake() {
        for (int i=0; i<numberToPool; i++) {
            GameObject _obj = Instantiate(objectToPool, transform);
            _obj.SetActive(false);
            _obj.transform.position = transform.position;
            inactiveObjects.Add(_obj.GetInstanceID(), _obj);
        }
    }

    /// <summary>
    /// Activates a GameObject from the pool if available.
    /// Returns a reference to the GameObject if successful, null otherwise.
    /// </summary>
    public GameObject ActivateObject() 
    {
        if (inactiveObjects.Count <= 0) {
            Debug.LogError("Object limit reached");
            return null;
        }

        // All we need is an inactive object, it doesn't matter which one.
        // We access a random element in inactiveObjects using the first key in the list of keys.
        int _firstKey = new List<int>(inactiveObjects.Keys)[0];
        GameObject _obj = inactiveObjects[_firstKey];
        _obj.SetActive(true);
        inactiveObjects.Remove(_firstKey);
        activeObjects.Add(_firstKey, _obj);
        return _obj;
    }

    /// <summary>
    /// Deactivates a GameObject and returns it to the pool.
    /// </summary>
    public void DeactivateObject(GameObject _obj) {
        _obj.SetActive(false);

        int _key = _obj.GetInstanceID();
        activeObjects.Remove(_key);
        inactiveObjects.Add(_key, _obj);
        _obj.transform.position = new Vector3(0, -20, 0);
    }
}
