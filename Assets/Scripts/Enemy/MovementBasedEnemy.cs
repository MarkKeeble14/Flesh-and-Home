using System.Collections;
using UnityEngine;

public class MovementBasedEnemy : RoomEnemy
{
    protected Coroutine currentState;

    [Header("References")]
    [SerializeField] private EnemyMovement movement;
    public EnemyMovement Movement => movement;

    protected new void Start()
    {
        base.Start();
        movement.SetMove(false);
    }

    public override void Activate()
    {
        base.Activate();
        currentState = StartCoroutine(AttackCycle());
        movement.SetMove(true);
    }

    protected IEnumerator AttackCycle()
    {
        SetAttack();

        bool doneAttacking = false;
        while (!doneAttacking)
        {
            movement.SetDisabledForAttack(AttackIsDisablingMove || GetIsInRangeForNextAttack(movement.Target, attack.Key));

            // 
            if (attack.Key.CanAttack(movement.Target))
            {
                yield return StartCoroutine(StartAttack(movement.Target, false));
                doneAttacking = true;
            }
            yield return null;
        }

        StartCoroutine(AttackCycle());
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
