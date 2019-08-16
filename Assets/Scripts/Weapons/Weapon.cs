using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected bool triggerPressed;
    public float maxEnergy = 10;
    [HideInInspector] public float energy = 0;
    
    public virtual void PressTrigger(bool triggerPressedNow)
    {
        this.triggerPressed = triggerPressedNow;
    }
}
