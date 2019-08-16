using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeWeapon : Weapon
{
    [SerializeField] float chargeRate = 1f;
    [SerializeField] float chargeDelay = 0.1f;
    [SerializeField] AudioClip fireClip = null;
    /*
     * These two arrays (chargeLevels and projectiles) must be the same length.
     * They represent the projectiles that can be fired from this weapon depending on the current
     * amount of energy.
     * Each index of chargeLevels corresponds to an index of projectiles.
     * If the projectile is null, no projectile will be fired.
    */
    [SerializeField] float[] chargeLevels = null;
    [SerializeField] GameObject[] projectiles = null;

    Timer chargeTimer;

    protected bool isCharging;

    int chargeLevel {
        get {
            for (int i=0; i<chargeLevels.Length; i++) {
                if (energy < chargeLevels[i])
                    return i-1;
            }
            return chargeLevels.Length-1;
        }
        set {}
    }

    protected Animator animator = null;
    protected Animator particleAnimator = null;

    void Awake()
    {
        animator = GetComponent<Animator>();
        particleAnimator = transform.Find("Particles").GetComponent<Animator>();
        chargeTimer = new Timer();
        isCharging = true;
    }

    void Start()
    {
        if (projectiles.Length != chargeLevels.Length) {
            Debug.LogError("[ChargeWeapon] Projectiles and Charge Levels must be the same length.");
        }
    }

    void Update()
    {
        if (triggerPressed) {
            Fire();
        }

        if (isCharging) {
            Charge();
        }
        else {
            if (chargeTimer.isDone) {
                isCharging = true;
            }
        }

        animator.SetInteger("Charge Level", chargeLevel);
        particleAnimator.SetBool("Charging", isCharging && chargeLevel < chargeLevels.Length-1);
    }

    void Charge()
    {
        AddEnergy(chargeRate * Time.deltaTime);
    }

    void AddEnergy(float energyToAdd)
    {
        if (energy < maxEnergy) {
            energy += energyToAdd;
            if (energy > maxEnergy)
                energy = maxEnergy;
        }
    }
    
    public override void PressTrigger(bool triggerPressedNow)
    {
        base.PressTrigger(triggerPressedNow);
    }
    
    Projectile Fire()
    {
        if (chargeLevel < 0 || chargeLevel >= projectiles.Length) {
            return null;
        }

        GameObject projectilePrefab = projectiles[chargeLevel];
        if (projectilePrefab == null) {
            return null;
        }

        GameObject go = PoolController.Activate(projectilePrefab, transform.position, transform.rotation);
        if (go != null) {
            // Successfully shot!!!
            energy = 0;
            chargeTimer.SetTime(chargeDelay);
            isCharging = false;
            AudioController.Instance.PlayOneShot(fireClip);
            return go.GetComponent<Projectile>();
        }
        else
            return null;
    }
}
