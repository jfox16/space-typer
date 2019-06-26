using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Projectile.Type projectileType;
    public float shootDelay = 1.0f;
    
    public void Fire() {
        Projectile _proj = GameController.ActivateProjectile(projectileType).GetComponent<Projectile>();
        if (_proj != null) {
            _proj.transform.position = transform.position;
            _proj.moveDirection = Vector2.up;
        }
    }
}
