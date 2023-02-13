using UnityEngine;

public abstract class RangeBasedAttacker : Attacker
{
    [SerializeField] private float attackRange;

    public override bool CanAttack(Transform target)
    {
        if (target == null)
        {
            // Debug.Log("Can't Attack: Target is null");
            return false;
        }
        if (GetDistanceToTransform(target) > attackRange)
        {
            // Debug.Log("Can't Attack: Not in Range");
            return false;
        }
        return base.CanAttack(target);
    }
}
