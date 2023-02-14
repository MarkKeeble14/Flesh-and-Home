using System.Collections;
using UnityEngine;

public class InstaDamageAttack : Attack
{
    [Header("Specific Settings")]
    [SerializeField] private float damage;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(damage);
        }

        yield return null;
    }
}
