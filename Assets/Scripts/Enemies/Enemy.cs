using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    //=============================================================================================

    #region VARIABLES

    public float maxHealth = 10;
    [SerializeField] float moveSpeed = 5.0f;

    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new CircleCollider2D collider;
    [HideInInspector] public Animator animator;

    [SerializeField] AudioClip hurtClip = null;
    [SerializeField] AudioClip dieClip = null;

    #endregion

    //=============================================================================================

    #region UNITY CALLBACKS

    protected virtual void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        isAlive = true;
        health = maxHealth;
        animator.SetBool("Is Alive", true);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isAlive) {
            Move();
        }
    }

    #endregion

    //=============================================================================================

    #region PRIVATE METHODS

    void Move() 
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < GameController.Instance.yBoundMin-5) {
            Deactivate();
        }
    }

    void Die() 
    {
        isAlive = false;
        animator.SetBool("Is Alive", false);
    }

    #endregion

    //=============================================================================================

    #region PUBLIC METHODS

    public override void Hurt(float damage) 
    {
        health -= damage;
        if (health <= 0) Die();
        else animator.SetTrigger("Hurt");
    }

    public void Deactivate()
    {
        PoolController.Deactivate(gameObject);
    }

    public void PlayHurtClip()
    {
        if (hurtClip != null) {
            AudioController.Instance.PlayOneShot(hurtClip);
        }
    }

    public void PlayDieClip()
    {
        if (dieClip != null) {
            AudioController.Instance.PlayOneShot(dieClip);
        }
    }

    #endregion

    //=============================================================================================
}
