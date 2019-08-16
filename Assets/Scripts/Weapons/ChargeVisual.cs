using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeVisual : MonoBehaviour
{
    Animator animator = null;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AnimateChargeLevel(int chargeLevel)
    {
        animator.SetInteger("Charge Level", chargeLevel);
    }
}
