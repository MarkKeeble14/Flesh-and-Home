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
    [SerializeField] private LayerMask roomTriggerMask;
    [SerializeField] private LayerMask groundMask;

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
            float groundCheckRange = 2.5f;
            // Debug.Log(name + ", Chose WanderPos: " + wanderPos);

            // Check to make sure wander pos is valid
            // Chose an invalid spot i.e., one that is outside of a room (1st condition) or is not on any ground (2nd condition)
            if (Physics.OverlapSphere(wanderPos, transform.localScale.x, roomTriggerMask).Length == 0)
            {
                // return to retry
                return;
            }
            if (!Physics.Raycast(wanderPos + Vector3.up, Vector3.down, groundCheckRange, groundMask))
            {
                // return to retry
                return;
            }

            // Debug.Log("Success With WanderPos: " + wanderPos);
            enemy.Movement.SetMove(true);
            enemy.Movement.ClearTargets();
            enemy.Movement.OverrideTarget(wanderPos, .25f, true, () => hasReachedWanderPos = true, null);
            timer = 0;
            hasReachedWanderPos = false;
            hasSetWanderPos = true;
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
