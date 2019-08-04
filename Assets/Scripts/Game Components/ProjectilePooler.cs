using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] bool debug = false;

    public static ProjectilePooler Instance = null;

    [SerializeField] GameObject objectToPool = null;
    [SerializeField] int numberToPool = 1;

    Dictionary<int, Projectile> inactiveProjectiles = new Dictionary<int, Projectile>();
    Dictionary<int, Projectile> activeProjectiles = new Dictionary<int, Projectile>();


    void Awake() {
        Instance = this;
    }

    void Start() {
        for (int i=0; i<numberToPool; i++) {
            GameObject _obj = Instantiate(objectToPool, transform);
            _obj.SetActive(false);
            inactiveProjectiles.Add(_obj.GetInstanceID(), _obj.GetComponent<Projectile>());
        }
    }

    public Projectile ActivateProjectile() {
        if (inactiveProjectiles.Count == 0) {
            Debug.LogError("ProjectilePooler: Projectile limit reached.");
            return null;
        }
        // Get random inactive projectile
        int _key = new List<int>(inactiveProjectiles.Keys)[0];
        Projectile _proj;
        inactiveProjectiles.TryGetValue(_key, out _proj);
        inactiveProjectiles.Remove(_key);
        activeProjectiles.Add(_key, _proj);
        _proj.gameObject.SetActive(true);
        
        return _proj;
    } 

    public void DeactivateProjectile(Projectile _proj) {
        int _key = _proj.gameObject.GetInstanceID();
        activeProjectiles.Remove(_key);
        inactiveProjectiles.Add(_key, _proj);
        _proj.gameObject.SetActive(false);
    }
}
