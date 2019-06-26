using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public enum Team {Neutral, Player, Enemy}
    public Team team;
    public bool isAlive = true;
    public abstract void Hurt(float damage);
}
