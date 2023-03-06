using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    public enum EnemyStateType
    {
        IDLE,
        ATTACK,
        DEAD,
    }

    public virtual void EnterState(RoomEnemyStateController enemy)
    {
        // Debug.Log(name + ", Entering State: " + this);
    }
    public virtual void ExitState(RoomEnemyStateController enemy)
    {
        // Debug.Log(name + ", Exiting State: " + this);
    }
    public virtual void UpdateState(RoomEnemyStateController enemy)
    {
        // 
    }
}
