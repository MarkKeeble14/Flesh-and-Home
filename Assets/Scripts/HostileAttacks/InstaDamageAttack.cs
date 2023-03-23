using System.Collections;
using UnityEngine;

public class InstaDamageAttack : Attack
{
    [Header("Specific Settings")]
    [SerializeField] private float damage;

    public override void Boost()
    {
        throw new System.NotImplementedException();
        // base.Boost();
    }

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(damage, DamageSource.ENEMY_SWARM);
        }

        yield return null;
    }
}
