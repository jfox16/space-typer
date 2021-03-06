using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingWeapon : MonoBehaviour
{
    [SerializeField] GameObject shotPrefab;
    [SerializeField] AudioClip shotClip;
    [SerializeField] GameObject killShotPrefab;
    [SerializeField] AudioClip killShotClip;
    [SerializeField] float shootDelay = 0.05f;

    public void Shoot(TypingEnemy target)
    {
        Shoot(target, false);
    }

    public void ShootKillShot(TypingEnemy target)
    {
        Shoot(target, true);
    }

    public void Shoot(TypingEnemy target, bool isKillShot)
    {
        StartCoroutine(ShootAfterDelay(target, isKillShot));
    }

    IEnumerator ShootAfterDelay(TypingEnemy target, bool isKillShot)
    {
        yield return new WaitForSeconds(shootDelay);

        GameObject projectilePrefab = (isKillShot) ? killShotPrefab : shotPrefab;

        GameObject projectileGo = PoolController.Activate(
            projectilePrefab,
            transform.position,
            transform.rotation
        );
        TypingProjectile projectile = projectileGo.GetComponent<TypingProjectile>();
        projectile.Initialize( target, isKillShot );

        AudioClip clip = (isKillShot) ? killShotClip : shotClip;
        AudioController.Instance.PlayOneShot( clip );
    }
}
