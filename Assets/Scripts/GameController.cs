using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public float xBoundMin = -12.5f;
    public float xBoundMax = 12.5f;
    public float yBoundMin = -10.0f;
    public float yBoundMax = 10.0f;

    public ObjectPooler enemyPooler; 
    public ObjectPooler laserPooler;
    public ObjectPooler shurikenPooler;

    void Start() {
        
    }

    void Awake() {
        Instance = this;
    }

    public static GameObject ActivateProjectile(Projectile.Type projectileType) 
    {
        GameObject _projObj = null;
        switch(projectileType) 
        {
            case Projectile.Type.Laser:
                _projObj = Instance.laserPooler.ActivateObject();
                break;
            case Projectile.Type.Shuriken:
                _projObj = Instance.shurikenPooler.ActivateObject();
                break;
        }
        return _projObj;
    }

    public static void DeactivateProjectile(GameObject obj, Projectile.Type projectileType)
    {
        switch(projectileType) 
        {
            case Projectile.Type.Laser:
                Instance.laserPooler.DeactivateObject(obj);
                break;
            case Projectile.Type.Shuriken:
                Instance.shurikenPooler.DeactivateObject(obj);
                break;
        }
    }
}
