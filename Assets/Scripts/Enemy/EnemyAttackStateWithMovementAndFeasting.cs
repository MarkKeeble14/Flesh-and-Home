using System.Collections;
using UnityEngine;

public class EnemyAttackStateWithMovementAndFeasting : EnemyFeastState
{
    [SerializeField] private bool prioritizeFeasting;

    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);
    }

    public override void ExitState(RoomEnemyStateController enemy)
    {
        base.ExitState(enemy);

        enemy.Stop();

        enemy.Movement.SetDisabledForAttack(false);
    }

    public override IEnumerator StateBehaviour(RoomEnemyStateController enemy)
    {
        Attack attack = enemy.SetAttack();

        bool doneAttacking = false;
        while (!doneAttacking)
        {
            if (enemy.IsDead)
            {
                // Debug.Log("Tried to Attack When Dead");
                yield break;
            }

            if (prioritizeFeasting
                && enemyFeastStateController.CurrentTarget != null)
            {
                enemy.SwitchState(EnemyStateType.FEAST);
            }

            enemy.Movement.SetMove(true);
            enemy.Movement.SetDisabledForAttack(enemy.AttackIsDisablingMove || GetIsInRangeForNextAttack(enemy.Movement.Target, attack));

            // 
            // Debug.Log("Checking if Can Attack");
            if (attack.CanAttack(enemy.Movement.Target))
            {
                // Debug.Log("Can Attack; Starting Coroutine");
                // Debug.Log("Can Attack");
                yield return StartCoroutine(enemy.StartAttack(enemy.Movement.Target, false));
                doneAttacking = true;
                // Debug.Log("Done Coroutine");
            }
            else
            {
                // Debug.Log("Can't Attack");
            }
            yield return null;
        }

        // Debug.Log("Done StateBehaviour");

        StartCoroutine(StateBehaviour(enemy));
    }

    private bool GetIsInRangeForNextAttack(Transform target, Attack attack)
    {
        if (attack.HasMaxRange)
        {
            return attack.WithinRange(target);
        }
        return true;
    }
}
