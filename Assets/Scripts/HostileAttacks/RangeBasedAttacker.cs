using UnityEngine;

public abstract class RangeBasedAttacker : Attacker
{
    [SerializeField] private float attackRange;

    public override bool CanAttack(Transform target)
    {
        if (target == null) return false;
        if (GetDistanceToTransform(target) > attackRange) return false;
        return base.CanAttack(target);
    }
}
