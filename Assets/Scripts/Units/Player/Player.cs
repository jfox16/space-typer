using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance = null;

    [SerializeField] protected List<Weapon> weapons = new List<Weapon>();
    protected int equippedWeaponIndex = 0;

    public Weapon equippedWeapon {
        get {
            if (equippedWeaponIndex < weapons.Count)
                return weapons[equippedWeaponIndex];
            else
                return null;
        }
    }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }
}
