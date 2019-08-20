using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : Poolable
{
    //=============================================================================================

    #region VARIABLES

    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new Collider2D collider;

    // Teams
    public enum Team {Neutral, Player, Enemy}
    public Team team;

    // Materials
    public enum Material {Neutral, Metal, Energy, Void}
    public Material material;
    static float[,] materialModifierMap = null;
    static int numOfMaterials = 4;

    // Returns a vector pointing toward the front of this object.
    public Vector3 forwardVector
    {
        get { return transform.rotation * Vector3.right; }
        private set {}
    }

    public bool isAlive = false;

    // Stats
    public int maxHealth = 1;
    [HideInInspector] public int health = 0;
    public int baseDamage = 1;
    public int baseArmor = 0;
    public float moveSpeed = 1;

    protected Animator animator = null;
    protected Animator effectAnimator = null;

    // This variable is used to ensure that a Unit is only damaged once per frame
    // [HideInInspector] public bool hurtThisFrame = false;

    Dictionary<Unit, bool> unitsCollidedWithThisFrame = new Dictionary<Unit, bool>();

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
        // hurtThisFrame = false;
        unitsCollidedWithThisFrame.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D otherColl) 
    {
        // Check if alive
        if (!isAlive)
            return;

        Unit otherUnit = otherColl.GetComponent<Unit>();
        if (otherUnit != null) {
            CollideWith(otherUnit);
        }
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

    // Take damage
    public virtual int TakeDamage(int baseDamage, Material damageType) 
    {
        // Check if this unit is alive
        if (!isAlive)
            return -1;

        // Apply material modifiers
        int damage = (int)(baseDamage * LookUpMaterialModifier(damageType, this.material));
        if (damage <= 0)
            return -1;

        // Apply armor reduction
        damage = Mathf.Clamp(damage - baseArmor, 1, 255);

        // Deduct damage from health
        health -= damage;
        if (health <= 0)
            Die();

        return damage;
    }

    /**
     * This function is how all Units do "combat" with each other.
     * It makes sure they are valid targets, and if so, they deal damage to each other.
     * Additional damage calculation is done in the DealDamageTo and TakeDamage functions.
     */
    protected virtual void CollideWith(Unit otherUnit)
    {
        // Check if Units have not collided yet this frame
        bool haveNotCollided = !HaveCollidedThisFrame(this, otherUnit);
        // Check if both Units are alive
        bool bothAlive = (isAlive && otherUnit.isAlive);
        // bool _bothNotHurtThisFrame = (!hurtThisFrame && !otherUnit.hurtThisFrame);
        // Check if Units are on different teams
        bool notSameTeam = (team != otherUnit.team || otherUnit.team == Unit.Team.Neutral);

        if (bothAlive && haveNotCollided && notSameTeam) 
        {
            // Successful Collision!

            // Deal damage to each other
            int damageDealtToOther = this.DealDamageTo(otherUnit);
            int damageDealtToThis = otherUnit.DealDamageTo(this);

            // Add units to each others' UnitsCollidedWithThisFrame dictionaries
            this.AddUnitCollidedWith(otherUnit);
            otherUnit.AddUnitCollidedWith(this);

            // Debug Log the details of collision
            if (showDebug && damageDealtToOther > 0 && damageDealtToThis > 0) {
                Debug.Log(string.Format("[Unit] Units collide! {0} takes {1} {2} damage, {3} takes {4} {5} damage.", 
                    gameObject.name, 
                    damageDealtToThis,
                    otherUnit.material,
                    otherUnit.gameObject.name,
                    damageDealtToOther,
                    material
                ));
            }
        }
    }

    // Keeps track of a unit as having been collided with this frame.
    public virtual void AddUnitCollidedWith(Unit otherUnit) 
    {
        unitsCollidedWithThisFrame.Add(otherUnit, true);
    }

    // Returns true if these two units have already collided this frame.
    public static bool HaveCollidedThisFrame(Unit unit1, Unit unit2) 
    {
        if (unit1.unitsCollidedWithThisFrame.ContainsKey(unit2)
            || unit2.unitsCollidedWithThisFrame.ContainsKey(unit1)) 
        {
            return true;
        }
        else {
            return false;
        }
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

        int _neutral  = (int)Material.Neutral;
        int _metal    = (int)Material.Metal;
        int _energy   = (int)Material.Energy;
        int _void     = (int)Material.Void;

        materialModifierMap[_neutral, _neutral] = 1.0f;
        materialModifierMap[_neutral, _metal]   = 1.0f;
        materialModifierMap[_neutral, _energy]  = 1.0f;
        materialModifierMap[_neutral, _void]    = 0.0f;

        materialModifierMap[_metal, _neutral] = 1.5f;
        materialModifierMap[_metal, _metal]   = 1.0f;
        materialModifierMap[_metal, _energy]  = 1.0f;
        materialModifierMap[_metal, _void]    = 0.0f;

        materialModifierMap[_energy, _neutral] = 1.0f;
        materialModifierMap[_energy, _metal]   = 1.0f;
        materialModifierMap[_energy, _energy]  = 0.0f;
        materialModifierMap[_energy, _void]    = 1.0f;

        materialModifierMap[_void, _neutral] = 1.0f;
        materialModifierMap[_void, _metal]   = 1.5f;
        materialModifierMap[_void, _energy]  = 0.0f;
        materialModifierMap[_void, _void]    = 1.5f;
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
