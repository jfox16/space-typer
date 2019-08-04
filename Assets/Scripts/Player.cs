using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    //=============================================================================================

    #region VARIABLES
    
    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new CircleCollider2D collider;
    private Animator animator = null;
    private Animator effectAnimator = null;
    
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float invulnTime = 3f;

    [SerializeField] Vector2 spawnPosition = new Vector2(0, -8);
    [SerializeField] List<Weapon> weapons = new List<Weapon>();
    int equippedWeaponIndex = 0;

    [SerializeField] AudioClip hurtClip = null;
    [SerializeField] AudioClip dieClip = null;
    
    Timer shootTimer, spawnTimer, invulnTimer;

    #endregion

    //=============================================================================================

    #region MONOBEHAVIOUR CALLBACKS

    void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        effectAnimator = transform.Find("Sprite").GetComponent<Animator>();

        shootTimer = new Timer();
        spawnTimer = new Timer();
        invulnTimer = new Timer();
    }

    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        if (isAlive) {
            Move();
            SwitchWeapon();
            Shoot();

            if (Input.GetButton("Cancel")) {
                Die();
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D col) {
        if (isAlive && invulnTimer.isDone) {
            Unit _unit = col.GetComponent<Unit>();
            if (_unit != null) {
                bool _isEnemy = (_unit.team == Unit.Team.Neutral || _unit.team != team);
                if (_unit.isAlive && _isEnemy ) {
                    Die();
                }
            }
        }
    }

    #endregion

    //=============================================================================================

    #region PRIVATE METHODS

    void CheckState()
    {
        effectAnimator.SetBool("Transparent", !invulnTimer.isDone);
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
        animator.SetFloat("Horizontal Movement", Input.GetAxis("Horizontal"));
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
                AudioController.Instance.PlayOneShot(_fireClip);
            }
        }
    }

    void Spawn()
    {
        isAlive = true;
        transform.position = spawnPosition;
        effectAnimator.SetBool("Transparent", true);
        animator.SetTrigger("Spawn");
        invulnTimer.SetTime(invulnTime);
    }

    void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        Invoke("Spawn", 1);
    }

    #endregion

    //=============================================================================================

    #region PUBLIC METHODS

    public override void Hurt(float damage) 
    {
        if (invulnTimer.isDone) {
            health -= damage;
            if (health <= 0) Die();
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
