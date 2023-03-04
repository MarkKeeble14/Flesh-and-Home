using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    public enum EnemyStateType
    {
        ATTACK,
        FEAST,
        DEAD,
    }

    private Coroutine stateBehaviour;

    public virtual void EnterState(RoomEnemyStateController enemy)
    {
        // Start State Behaviour
        stateBehaviour = StartCoroutine(StateBehaviour(enemy));
    }
    public virtual void ExitState(RoomEnemyStateController enemy)
    {
        if (stateBehaviour == null) return;

        // End State Behaviour
        StopCoroutine(stateBehaviour);
    }
    public virtual void UpdateState(RoomEnemyStateController enemy)
    {
        //
    }
    public abstract IEnumerator StateBehaviour(RoomEnemyStateController enemy);
}
