using System.Collections;
using UnityEngine;

public class EnemyAttackStateWithMovement : EnemyState
{
    private Attack attack;
    private bool hasSetAttack;
    private bool doneAttacking;
    private bool isAttacking;

    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        hasSetAttack = false;
    }

    public override void ExitState(RoomEnemyStateController enemy)
    {
        base.ExitState(enemy);
        enemy.Movement.SetDisabledForAttack(false);
    }


    public override void UpdateState(RoomEnemyStateController enemy)
    {
        base.UpdateState(enemy);

        if (!hasSetAttack)
        {
            attack = enemy.SetAttack();

            enemy.Movement.SetMove(true);
            enemy.Movement.ClearTargets();
            enemy.Movement.OverrideTarget(GameManager._Instance.PlayerAimAt, 0f, true, false, null, null);

            doneAttacking = false;
            hasSetAttack = true;
        }

        if (!doneAttacking)
        {
            enemy.Movement.SetDisabledForAttack(enemy.AttackIsDisablingMove || GetIsInRangeForNextAttack(enemy.Movement.Target, attack));

            if (isAttacking)
            {
                return;
            }
            if (attack.CanAttack(enemy.Movement.Target))
            {
                // Debug.Log("Can Attack");
                isAttacking = true;

                StartCoroutine(enemy.StartAttack(enemy.Movement.Target, false, delegate { isAttacking = false; doneAttacking = true; }));
            }
            return;
        }

        hasSetAttack = false;
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
