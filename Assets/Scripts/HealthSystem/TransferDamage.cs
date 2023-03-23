using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private EndableEntity damageable;

    public void Damage(float damage, DamageSource source)
    {
        damageable.Damage(damage, source);
    }

    public void Damage(float damage, Vector3 force, DamageSource source)
    {
        damageable.Damage(damage, force, source);
    }
}
