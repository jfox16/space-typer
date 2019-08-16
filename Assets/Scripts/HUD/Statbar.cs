using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statbar : MonoBehaviour
{
    enum Type {None, PlayerHealth, PlayerWeaponEnergy}
    [SerializeField] Type type = Type.None;
    [SerializeField] float maxChangeSpeed = 0.1f;
    GameObject spriteMask = null;

    void Awake()
    {
        spriteMask = transform.Find("Mask").gameObject;
    }

    void Update()
    {
        float value = 0;
        float maxValue = 1;

        switch(type)
        {
            case Type.PlayerHealth:
                Player player = Player.Instance;
                value = player.health;
                maxValue = player.maxHealth;
                break;
            case Type.PlayerWeaponEnergy:
                Weapon playerWeapon = Player.Instance.equippedWeapon;
                if (playerWeapon == null)
                    break;
                value = playerWeapon.energy;
                maxValue = playerWeapon.maxEnergy;
                break;
            default:
                break;
        }

        Vector3 targetMaskScale = new Vector3(value/maxValue, 1, 1);
        Vector3 maskScale = Vector3.MoveTowards(spriteMask.transform.localScale, targetMaskScale, maxChangeSpeed);
        spriteMask.transform.localScale = maskScale;
    }
}
