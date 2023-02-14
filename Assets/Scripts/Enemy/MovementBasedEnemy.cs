using UnityEngine;

public class MovementBasedEnemy : BasicEnemy
{
    [Header("References")]
    [SerializeField] private EnemyMovement movement;
    public EnemyMovement Movement => movement;

    private void Update()
    {
        if (nextAttack != null)
        {
            movement.SetDisabledForAttack(AttackIsDisablingMove || GetIsInRangeForNextAttack(movement.Target));

            // 
            if (nextAttack.CanAttack(movement.Target))
            {
                StartCoroutine(StartNextAttack(movement.Target));
            }
        }
    }

    private bool GetIsInRangeForNextAttack(Transform target)
    {
        if (nextAttack.HasMaxRange)
        {
            return nextAttack.WithinRange(target);
        }
        return true;
    }
}
