using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : Poolable
{
    protected Animator animator;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Spawn()
    {
        animator.SetTrigger("Spawn");
    }
}
