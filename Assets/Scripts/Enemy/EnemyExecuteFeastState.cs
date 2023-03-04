using System.Collections;
using UnityEngine;

public class EnemyExecuteFeastState : EnemyFeastState
{
    [Header("State Settings")]
    [SerializeField] private EnemyStateType playerNotInRoomOnEndChangeToState;
    [SerializeField] private EnemyStateType playerInRoomOnEndChangeToState;

    [SerializeField] private Transform dummyPoint;
    [SerializeField] private float pauseTime = 3f;
    [SerializeField] private float wanderDistance = 3f;
    [SerializeField] private bool roomWideFeastableDetection = true;

    [Header("Feasting Settings")]
    [SerializeField] private float feastTime;

    [Header("Execute Feast Stats")]
    [Tooltip("Additive")]
    [SerializeField] private float onFeastSizeChange;
    [Tooltip("Multiplicative")]
    [SerializeField] private float onFeastHPChange;

    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        enemyFeastStateController.HasRoomWideFeastableDetection = roomWideFeastableDetection;

        if (enemyFeastStateController.CurrentTarget == null)
            enemyFeastStateController.FindNewTarget();

        if (enemyFeastStateController.CurrentTarget == null)
            StartCoroutine(Wander(enemy));
        else
            StartCoroutine(TryFeast(enemy));
    }

    public override void ExitState(RoomEnemyStateController enemy)
    {
        base.ExitState(enemy);

        StopAllCoroutines();

        enemyFeastStateController.HasRoomWideFeastableDetection = false;
    }

    private IEnumerator Wander(RoomEnemyStateController enemy)
    {
        // Debug.Log("Started Override");
        Transform targetPoint = Instantiate(dummyPoint, transform.position + RandomHelper.RandomOffset(wanderDistance, 0, wanderDistance), Quaternion.identity);

        bool wandering = true;
        enemy.Movement.SetMove(true);

        // bool reachedPoint = false;
        Coroutine moving = StartCoroutine(enemy.Movement.GoToOverridenTarget(targetPoint, 1f,
            false,
            true,
            false,
            true,
            delegate
            {
                // reachedPoint = true;
                enemy.Movement.SetMove(false);
                wandering = false;
                Debug.Log("End Override: Success (Wander)");
            }, delegate
            {
                enemy.Movement.SetMove(false);
                wandering = false;
                Debug.Log("End Override: Failure (Wander)");
            }
        ));

        while (wandering)
        {
            if (enemyFeastStateController.CurrentTarget != null)
            {
                StopCoroutine(moving);
                StartCoroutine(TryFeast(enemy));
                yield break;
            }
            yield return null;
        }

        enemy.Movement.SetMove(false);
        // Wait a certain amount of time on end
        for (float t = 0; t < pauseTime; t += Time.deltaTime)
        {
            if (enemyFeastStateController.CurrentTarget == null)
                enemyFeastStateController.FindNewTarget();

            if (enemyFeastStateController.CurrentTarget != null)
            {
                StartCoroutine(TryFeast(enemy));
                yield break;
            }
            yield return null;
        }

        StartCoroutine(Wander(enemy));
    }

    private IEnumerator TryFeast(RoomEnemyStateController enemy)
    {
        enemy.Movement.SetMove(true);
        bool traveling = true;
        Coroutine travelingCoroutine = StartCoroutine(
            enemy.Movement.GoToOverridenTarget(enemyFeastStateController.CurrentTarget.transform,
            transform.localScale.x,
            false,
            true,
            false,
            false,
            delegate
            {
                Debug.Log(name + " Reached Override Target (Execute)");
                enemy.Movement.SetMove(false);
                traveling = false;
            },
            delegate
            {
                Debug.Log(name + " Failed to Reach Override Target (Execute)");

                traveling = false;
                ChangeState(enemy);
            }
        ));

        while (traveling)
        {
            if (enemyFeastStateController.CurrentTarget == null)
            {
                StopCoroutine(travelingCoroutine);
                StartCoroutine(Wander(enemy));
                yield break;
            }
            yield return null;
        }

        // reached target
        enemy.Movement.SetMove(false);

        // Wait a few moments Feast on Target
        for (float t = 0; t < feastTime; t += Time.deltaTime)
        {
            if (enemyFeastStateController.CurrentTarget == null)
            {
                Debug.Log(name + " Enemy Died Before Feasting Could Complete");
                enemyFeastStateController.FindNewTarget();

                if (enemyFeastStateController.CurrentTarget != null)
                    StartCoroutine(TryFeast(enemy));
                else
                    StartCoroutine(Wander(enemy));
                yield break;
            }
            yield return null;
        }

        // Feast on Target
        FeastOnTarget(enemyFeastStateController.CurrentTarget);

        ChangeState(enemy);
    }

    private void FeastOnTarget(FeastableEntity feastable)
    {
        feastable.FeastOn();
        if (feastable.TryGetComponent(out FeastEnemyStateController otherFeasted))
        {
            for (int i = 0; i <= otherFeasted.FeastLevel; i++)
            {
                OnFeast(feastable);
            }
        }
        else
        {
            // Debug.Log(name + " Feasted On " + currentTarget.name);
            OnFeast(feastable);
        }

    }

    public override IEnumerator StateBehaviour(RoomEnemyStateController enemy)
    {
        // 
        yield break;
    }

    private void ChangeState(RoomEnemyStateController enemy)
    {
        if (enemyFeastStateController.PlayerIsInRoom)
            enemy.SwitchState(playerInRoomOnEndChangeToState);
        else
            enemy.SwitchState(playerNotInRoomOnEndChangeToState);
    }

    private void OnFeast(FeastableEntity feastableEntity)
    {
        enemyFeastStateController.TargetScale += Vector3.one * onFeastSizeChange;
        if (onFeastHPChange != 0)
        {
            feastableEntity.MaxHealth *= onFeastHPChange;
        }
        feastableEntity.ResetHealth();
        enemyFeastStateController.FeastLevel++;
    }
}
