using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public bool isAlive = true;

    public enum Team {Neutral, Player, Enemy}
    public Team team;

    public float health;

    public virtual void Hurt(float damage) {
        health -= 1;
    }
}
