using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPCUnit
{
    //=============================================================================================

    #region VARIABLES

    [SerializeField] AudioClip hurtClip = null;
    [SerializeField] AudioClip dieClip = null;

    #endregion

    //=============================================================================================

    #region UNITY CALLBACKS

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    //=============================================================================================

    #region PRIVATE METHODS

    public override void Spawn()
    {
        base.Spawn();
        animator.SetTrigger("Spawn");
    }

    public override void Die() 
    {
        isAlive = false;
        animator.SetTrigger("Die");
    }

    #endregion

    //=============================================================================================

    #region PUBLIC METHODS

    public override int TakeDamage(int damage, Material damageType) 
    {
        damage = base.TakeDamage(damage, damageType);
        if (isAlive && damage > 0) 
            animator.SetTrigger("Hurt");
        return damage;
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
