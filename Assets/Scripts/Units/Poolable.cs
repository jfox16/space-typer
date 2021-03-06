using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    //=============================================================================================

    #region VARIABLES

    protected static bool showDebug = true;

    #endregion

    //=============================================================================================

    #region MONOBEHAVIOUR CALLBACKS

    protected virtual void OnEnable()
    {
        Spawn();
    }

    #endregion

    //=============================================================================================

    #region LIFECYCLE METHODS

    // For intializing things that need to reinitialize every time this Unit is respawned.
    public virtual void Spawn()
    {}

    public virtual void Die()
    {
        Deactivate();
    }

    protected virtual void Deactivate()
    {
        PoolController.Deactivate(gameObject);
    }

    #endregion

    //=============================================================================================
}
