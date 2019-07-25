using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public float maxHealth = 10;
    public float health;
    [SerializeField] float moveSpeed = 5.0f;
    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new CircleCollider2D collider;
    [HideInInspector] public Animator animator;
    AudioSource audioSource = null;
    [SerializeField] AudioClip hurtClip = null;
    [SerializeField] AudioClip dieClip = null;

    void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive) {
            Move();
        }
    }

    void Move() 
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < GameController.Instance.yBoundMin-5) {
            Deactivate();
        }
    }

    public override void Hurt(float damage) 
    {
        health -= damage;
        if (health <= 0) Die();
        else animator.SetTrigger("Hurt");
    }

    void Die() 
    {
        isAlive = false;
        animator.SetBool("Is Alive", false);
    }

    public void Activate()
    {
        isAlive = true;
        health = maxHealth;
        animator.SetBool("Is Alive", true);
    }

    public void Deactivate()
    {
        GameController.Instance.enemyPooler.DeactivateObject(gameObject);
    }

    public void PlayHurtClip()
    {
        if (hurtClip != null) {
            audioSource.PlayOneShot(hurtClip, AudioController.Instance.globalVolume);
        }
    }

    public void PlayDieClip()
    {
        if (dieClip != null) {
            audioSource.PlayOneShot(dieClip, AudioController.Instance.globalVolume);
        }
    }
}
