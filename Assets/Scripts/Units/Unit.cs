using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : Poolable
{
    //=============================================================================================

    #region VARIABLES

    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new Collider2D collider;

    public bool isAlive = false;

    public enum Team {Neutral, Player, Enemy}
    public Team team;

    public enum Material {Flesh, Metal, Energy, Void}
    public Material material;

    public Vector3 forwardVector
    {
        get {
            return transform.rotation * Vector3.right;
        }
        private set {}
    }

    [HideInInspector] public int health = 0;
    public int maxHealth = 1;
    public int baseDamage = 1;
    public int baseArmor = 0;
    public float moveSpeed = 1;

    static float[,] materialModifierMap = null;
    static int numOfMaterials = 4;

    protected Animator animator = null;
    protected Animator effectAnimator = null;

    // This variable is used to ensure that a Unit is only damaged once per frame
    [HideInInspector] public bool hurtThisFrame = false;

    #endregion

    //=============================================================================================

    #region MONOBEHAVIOUR CALLBACKS

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        effectAnimator = transform.Find("Sprite").GetComponent<Animator>();
    }

    protected virtual void Start()
    {}

    protected virtual void Update()
    {
        if (isAlive) {
            Move();
        }
    }

    protected virtual void LateUpdate()
    {
        hurtThisFrame = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D otherColl) 
    {
        // Check if alive
        if (!isAlive)
            return;

        Unit _otherUnit = otherColl.GetComponent<Unit>();
        if (_otherUnit == null)
            return;

        // Deal contact damage to both Units
        TradeDamage(_otherUnit);
    }

    #endregion

    //=============================================================================================

    #region LIFECYCLE METHODS

    // For intializing things that need to reinitialize every time this Unit is respawned.
    public override void Spawn() 
    {
        isAlive = true;
        health = maxHealth;
    }

    public override void Die()
    {
        isAlive = false;
        Deactivate();
    }

    protected override void Deactivate()
    {
        isAlive = false;
        base.Deactivate();
    }

    protected virtual void Move()
    {
        transform.position += forwardVector * moveSpeed * Time.deltaTime;

        if (CheckMovingOutOfBounds(3.0f)) {
            Deactivate();
        }
    }

    #endregion

    //=============================================================================================

    #region COMBAT METHODS

    // Deals collision damage to a target Unit
    public virtual int DealDamageTo(Unit targetUnit)
    {
        return targetUnit.TakeDamage(baseDamage, material);
    }

    protected virtual void TradeDamage(Unit otherUnit)
    {
        bool _bothAlive = (isAlive && otherUnit.isAlive);
        bool _bothNotHurtThisFrame = (!hurtThisFrame && !otherUnit.hurtThisFrame);
        bool _notMyTeam = (team != otherUnit.team || otherUnit.team == Unit.Team.Neutral);

        if (_bothAlive && _bothNotHurtThisFrame && _notMyTeam) 
        {
            int _damageDealtToOther = this.DealDamageTo(otherUnit);
            int _damageDealtToThis = otherUnit.DealDamageTo(this);

            if (showDebug && _damageDealtToOther > 0 && _damageDealtToThis > 0) {
                Debug.Log(string.Format("[Unit] Trade! {0} takes {1} {2} damage, {3} takes {4} {5} damage.", 
                    gameObject.name, 
                    _damageDealtToThis,
                    otherUnit.material,
                    otherUnit.gameObject.name,
                    _damageDealtToOther,
                    material
                ));
            }
        }
    }

    // Take damage
    public virtual int TakeDamage(int damage, Material damageType) 
    {
        // Check if this unit is alive
        if (!isAlive)
            return -1;

        // Apply material modifiers
        damage = (int)(damage * LookUpMaterialModifier(damageType, this.material));
        if (damage <= 0)
            return -1;
        else
            hurtThisFrame = true;

        // Apply armor reduction
        damage = Mathf.Clamp(damage - baseArmor, 1, 255);

        // Deduct damage from health
        health -= damage;
        if (health <= 0)
            Die();

        return damage;
    }

    #endregion

    //=============================================================================================

    #region UTILITY METHODS

    public void TurnThisTowards(Vector3 targetPosition)
    {
        Vector3 thisToTarget = targetPosition - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, thisToTarget);
    }

    // CheckOutOfBounds() returns true if this Unit is out of bounds, false otherwise.
    // Will return false if Unit is within extraMargin meters of bounds.
    protected bool CheckOutOfBounds()
    {
        return CheckOutOfBounds(0);
    }
    protected bool CheckOutOfBounds(float extraMargin)
    {
        if (transform.position.y + extraMargin < Global.yBoundMin)
            return true;
        else if (transform.position.y - extraMargin > Global.yBoundMax)
            return true;
        else if (transform.position.x + extraMargin < Global.xBoundMin)
            return true;
        else if (transform.position.x - extraMargin > Global.xBoundMax)
            return true;
        else
            return false;
    }

    // CheckMovingOutOfBounds() returns true if this Unit is moving out of bounds, false otherwise.
    // Returns false if Unit is moving inward from out of bounds.
    // Returns false if Unit is within extraMargin meters of bounds.
    protected bool CheckMovingOutOfBounds()
    {
        return CheckMovingOutOfBounds(0);
    }
    protected bool CheckMovingOutOfBounds(float extraMargin)
    {
        float angleOfRotation = transform.rotation.eulerAngles.z;

        // Going down or up
        if (angleOfRotation > 180.0f && angleOfRotation < 360.0f) {
            if (transform.position.y + extraMargin < Global.yBoundMin)
                return true;
        }
        else {
            if (transform.position.y - extraMargin > Global.yBoundMax)
                return true;
        }

        // Going left or right
        if (angleOfRotation > 90.0f && angleOfRotation < 270.0f) {
            if (transform.position.x + extraMargin < Global.xBoundMin)
                return true;
        }
        else {
            if (transform.position.x - extraMargin > Global.xBoundMax)
                return true;
        }

        return false;
    }

    static void GenerateMaterialModifierMap() 
    {
        materialModifierMap = new float[numOfMaterials, numOfMaterials];

        int _flesh  = (int)Material.Flesh;
        int _metal  = (int)Material.Metal;
        int _energy = (int)Material.Energy;
        int _void   = (int)Material.Void;

        materialModifierMap[_flesh, _flesh]  = 1.0f;
        materialModifierMap[_flesh, _metal]  = 1.0f;
        materialModifierMap[_flesh, _energy] = 1.0f;
        materialModifierMap[_flesh, _void]   = 0.0f;

        materialModifierMap[_metal, _flesh]  = 1.5f;
        materialModifierMap[_metal, _metal]  = 1.0f;
        materialModifierMap[_metal, _energy] = 1.0f;
        materialModifierMap[_metal, _void]   = 0.0f;

        materialModifierMap[_energy, _flesh]  = 1.5f;
        materialModifierMap[_energy, _metal]  = 1.0f;
        materialModifierMap[_energy, _energy] = 0.0f;
        materialModifierMap[_energy, _void]   = 1.0f;

        materialModifierMap[_void, _flesh]  = 1.0f;
        materialModifierMap[_void, _metal]  = 1.5f;
        materialModifierMap[_void, _energy] = 0.0f;
        materialModifierMap[_void, _void]   = 1.5f;
    }

    public static float LookUpMaterialModifier(Material damageType, Material targetMaterial) 
    {
        if (materialModifierMap == null) {
            GenerateMaterialModifierMap();
            Debug.Log("[Unit] Generating Material Modifier Map.");
        }
        return materialModifierMap[(int)damageType, (int)targetMaterial];
    }

    #endregion

    //=============================================================================================
}
