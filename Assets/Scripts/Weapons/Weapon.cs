using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    public float shootDelay = 1.0f;
    public AudioClip fireClip = null;
    
    public Projectile Fire() {
        GameObject _projGo = PoolController.Activate(projectile, transform.position, Quaternion.identity);
        return _projGo.GetComponent<Projectile>();
    }
}
