using System.Collections;
using UnityEngine;

public class EnemyAttackStateWithMovement : EnemyState
{
    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        enemy.Movement.SetMove(true);
    }

    public override void ExitState(RoomEnemyStateController enemy)
    {
        base.ExitState(enemy);

        enemy.Movement.SetDisabledForAttack(false);
    }

    public override IEnumerator StateBehaviour(RoomEnemyStateController enemy)
    {
        Attack attack = enemy.SetAttack();

        bool doneAttacking = false;
        while (!doneAttacking)
        {
            enemy.Movement.SetDisabledForAttack(enemy.AttackIsDisablingMove || GetIsInRangeForNextAttack(enemy.Movement.Target, attack));

            // 
            // Debug.Log("Checking if Can Attack");
            if (attack.CanAttack(enemy.Movement.Target))
            {
                // Debug.Log("Can Attack");
                yield return StartCoroutine(enemy.StartAttack(enemy.Movement.Target, false));
                doneAttacking = true;
            }
            yield return null;
        }

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
