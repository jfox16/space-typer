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
    AudioSource audioSource;

    [SerializeField] Vector2 spawnPosition = new Vector2(0, -8);
    [SerializeField] List<Weapon> weapons;
    int equippedWeaponIndex = 0;
    Timer shootTimer;
    Timer spawnTimer;

    void Start() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shootTimer = new Timer();
        spawnTimer = new Timer();

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
    
    void OnTriggerEnter2D(Collider2D col) {
        Unit _unit = col.GetComponent<Unit>();
        if (_unit != null) {
            bool _isEnemy = (_unit.team == Unit.Team.Neutral || _unit.team != team);
            if (_unit.isAlive && this.isAlive && _isEnemy ) {
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
        if (Input.GetButtonDown("Weapon 1")) {
            equippedWeaponIndex = 0;
        }
        else if (Input.GetButtonDown("Weapon 2")) {
            equippedWeaponIndex = 1;
        }
        else if (Input.GetButtonDown("Previous Weapon")) {
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
            // Play audio
            AudioClip _fireClip = _equippedWeapon.fireClip;
            if (_fireClip != null) {
                audioSource.PlayOneShot(_fireClip, 0.1f);
            }
        }
    }

    public override void Hurt(float damage) 
    {
        health -= damage;
        if (health <= 0) Die();
    }

    void Spawn()
    {
        isAlive = true;
        transform.position = spawnPosition;
        animator.SetTrigger("Spawn");
    }

    void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        Invoke("Spawn", 1);
    }
}
