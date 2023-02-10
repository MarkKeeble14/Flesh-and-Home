using System.Collections;
using UnityEngine;

public class InstaDamageAttacker : RangeBasedAttacker
{
    [Header("Specific Settings")]
    [SerializeField] private float damage;

    protected override IEnumerator Attack(Transform target)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(damage);
        }

        yield return null;
    }
}
