using System.Collections;
using UnityEngine;

public class RoomEnemyWanderState : EnemyState
{
    [SerializeField] private float wanderDistance = 5f;

    [SerializeField] private float pauseTime = 1f;

    private Vector3 wanderPos;
    private bool hasSetWanderPos;
    private bool hasReachedWanderPos;
    private float timer;
    private int stuckTracker;
    private Vector3 lastPosition;

    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        hasSetWanderPos = false;

    }

    public override void ExitState(RoomEnemyStateController enemy)
    {
        base.ExitState(enemy);

        enemy.Movement.SetDisabledForAttack(false);
    }

    public override void UpdateState(RoomEnemyStateController enemy)
    {
        base.UpdateState(enemy);

        if (enemy.PlayerIsInRoom)
        {
            enemy.SwitchState(EnemyStateType.ATTACK);
            return;
        }

        if (transform.position == lastPosition && !hasReachedWanderPos)
        {
            stuckTracker++;
            if (stuckTracker > 60)
            {
                hasSetWanderPos = false;
            }
        }
        else
        {
            stuckTracker = 0;
        }

        if (!hasSetWanderPos)
        {
            wanderPos = transform.position + RandomHelper.RandomOffset(wanderDistance, 0, wanderDistance);
            enemy.Movement.SetMove(true);
            enemy.Movement.ClearTargets();
            enemy.Movement.OverrideTarget(wanderPos, .25f, true, () => hasReachedWanderPos = true, null);
            hasSetWanderPos = true;
            hasReachedWanderPos = false;
            timer = 0;
        }

        if (!hasReachedWanderPos)
        {
            lastPosition = transform.position;
            return;
        }

        if (timer < pauseTime)
        {
            timer += Time.deltaTime;
            return;
        }

        hasSetWanderPos = false;
    }
}
