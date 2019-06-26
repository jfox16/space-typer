using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    [SerializeField] float health = 1f;
    [SerializeField] float moveSpeed = 1f;
    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new CircleCollider2D collider;
    Animator animator;

    [SerializeField] List<Weapon> weapons;
    int equippedWeaponIndex = 0;
    Timer shootTimer;

    void Start() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        shootTimer = new Timer();

        team = Unit.Team.Player;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive) {
            Move();
            SwitchWeapon();
            Shoot();

            if (Input.GetButton("Cancel")) {
                Die();
            }
        }
    }

    void Move() 
    {
        // Read Input
        float _xInput = Input.GetAxisRaw("Horizontal");
        float _yInput = Input.GetAxisRaw("Vertical");
        float _newPositionX = transform.position.x + _xInput * moveSpeed * Time.deltaTime;
        float _newPositionY = transform.position.y + _yInput * moveSpeed * Time.deltaTime;

        // Check Horizontal Bounds
        if (_newPositionX - collider.radius < GameController.Instance.xBoundMin) {
            _newPositionX = GameController.Instance.xBoundMin + collider.radius;
        }
        else if (_newPositionX + collider.radius > GameController.Instance.xBoundMax) {
            _newPositionX = GameController.Instance.xBoundMax - collider.radius;
        }
        // Check Vertical Bounds
        if (_newPositionY - collider.radius < GameController.Instance.yBoundMin) {
            _newPositionY = GameController.Instance.yBoundMin + collider.radius;
        }
        else if (_newPositionY + collider.radius > GameController.Instance.yBoundMax) {
            _newPositionY = GameController.Instance.yBoundMax - collider.radius;
        }

        // Make final movement
        transform.position = new Vector3(_newPositionX, _newPositionY, 0);
        // Animate
        animator.SetFloat("Horizontal Movement", _xInput);
    }

    void SwitchWeapon() 
    {
        if (Input.GetButtonDown("Previous Weapon")) {
            equippedWeaponIndex = (equippedWeaponIndex - 1 + weapons.Count) % weapons.Count;
        }
        else if (Input.GetButtonDown("Next Weapon")) {
            equippedWeaponIndex = (equippedWeaponIndex + 1) % weapons.Count;
        }
    }

    void Shoot() 
    {
        if (Input.GetButton("Fire1") && shootTimer.isDone && weapons.Count > 0) {
            Weapon _equippedWeapon = weapons[equippedWeaponIndex];
            _equippedWeapon.Fire();
            shootTimer.SetTime(_equippedWeapon.shootDelay);
        }
    }

    public override void Hurt(float damage) {
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
    }
}
