using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Player is the script that sets this GameObject as the player's. 
This script extends from Player and 

- Spaceship extends from Player.
- Player is an object that creates a singleton of itself (which can be accessed with the static 
variable Player.Instance)

 */
public class Spaceship : Player
{
    //=============================================================================================

    #region VARIABLES
    
    [HideInInspector] public CapsuleCollider2D capsuleCollider;
    
    [SerializeField] float invulnTime = 2.0f;

    [SerializeField] AudioClip hurtClip = null;
    [SerializeField] AudioClip dieClip = null;
    
    Timer spawnTimer, invulnTimer;

    #endregion

    //=============================================================================================

    #region MONOBEHAVIOUR CALLBACKS

    protected override void Awake() 
    {
        base.Awake();

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spawnTimer = new Timer();
        invulnTimer = new Timer();
    }

    // Update is called once per frame
    // START HERE TO FIGURE OUT WHAT A FUNCTION DOES
    protected override void Update()
    {
        CheckState();
        if (isAlive) 
        {
            Move();
            SwitchWeapon();
            Shoot();
        }
    }

    #endregion

    //=============================================================================================

    #region PRIVATE METHODS

    protected override void Move() 
    {
        // Read Input
        float _xInput = Input.GetAxis("Horizontal");
        float _yInput = Input.GetAxis("Vertical");
        // Camera Movement
        MainCameraTransformer.SetCenterRotation(new Vector3(0, _xInput, 0) * 0.3f);

        float _newPositionX = transform.position.x + _xInput * moveSpeed * Time.deltaTime;
        float _newPositionY = transform.position.y + _yInput * moveSpeed * Time.deltaTime;

        // Check Horizontal Bounds
        if (_newPositionX - capsuleCollider.size.x/2 < Global.xBoundMin) {
            _newPositionX = Global.xBoundMin + capsuleCollider.size.x/2;
        }
        else if (_newPositionX + capsuleCollider.size.x/2 > Global.xBoundMax) {
            _newPositionX = Global.xBoundMax - capsuleCollider.size.x/2;
        }
        // Check Vertical Bounds
        if (_newPositionY - capsuleCollider.size.y/2 < Global.yBoundMin) {
            _newPositionY = Global.yBoundMin + capsuleCollider.size.y/2;
        }
        else if (_newPositionY + capsuleCollider.size.y/2 > Global.yBoundMax) {
            _newPositionY = Global.yBoundMax - capsuleCollider.size.y/2;
        }

        // Make final movement
        transform.position = new Vector3(_newPositionX, _newPositionY, 0);
        // Animate
        animator.SetFloat("Horizontal Movement", Input.GetAxis("Horizontal"));
    }

    public override void Spawn()
    {
        base.Spawn();

        animator.SetTrigger("Spawn");
        effectAnimator.SetBool("Transparent", true);
        invulnTimer.SetTime(invulnTime);
    }

    public override void Die()
    {
        isAlive = false;
        Invoke("Spawn", 1);
        animator.SetTrigger("Die");
        MainCameraTransformer.AddVelocity(Random.insideUnitCircle.normalized * 5);

        // if(equippedWeapon) {
        //     equippedWeapon.enabled = false;
        // }
    }

    void CheckState()
    {
        effectAnimator.SetBool("Transparent", !invulnTimer.isDone);
    }

    void SwitchWeapon() 
    {
        if (weapons.Count <= 0) 
            return;
            
        // if (Input.GetButtonDown("Weapon 1")) {
        //     equippedWeaponIndex = 0;
        // }
        // else if (Input.GetButtonDown("Weapon 2")) {
        //     equippedWeaponIndex = 1;
        // }
        else if (Input.GetButtonDown("Previous Weapon")) {
            equippedWeaponIndex = (equippedWeaponIndex - 1 + weapons.Count) % weapons.Count;
        }
        else if (Input.GetButtonDown("Next Weapon")) {
            equippedWeaponIndex = (equippedWeaponIndex + 1) % weapons.Count;
        }
    }

    void Shoot() 
    {
        if (equippedWeapon != null) {
            equippedWeapon.PressTrigger(Input.GetButton("Fire1"));
        }
        else {
            Debug.LogError("[Spaceship] equippedWeapon is null!");
        }
    }

    #endregion

    //=============================================================================================

    #region PUBLIC METHODS

    public override int DealDamageTo(Unit targetUnit)
    {
        int damageToDeal = baseDamage;

        // Don't deal damage if Spaceship is still in invulnerable state
        if (!invulnTimer.isDone)
            damageToDeal = 0;

        return targetUnit.TakeDamage(damageToDeal, material);
    }

    public override int TakeDamage(int damage, Material damageType) 
    {
        if (isAlive && invulnTimer.isDone) {
            damage = base.TakeDamage(damage, damageType);
            if (damage > 0 && isAlive) {
                invulnTimer.SetTime(invulnTime);
                MainCameraTransformer.AddVelocity(Random.insideUnitCircle.normalized * 5);
                PlayHurtClip();
            }
            return damage;
        }
        else {
            return -1;
        }
    }
    
    public void PlayHurtClip()
    {
        AudioController.Instance.PlayOneShot(hurtClip);
    }

    public void PlayDieClip()
    {
        AudioController.Instance.PlayOneShot(dieClip);
    }

    #endregion

    //=============================================================================================
}
