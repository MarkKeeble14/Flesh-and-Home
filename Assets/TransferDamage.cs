using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private EndableEntity damageable;

    public void Damage(float damage)
    {
        damageable.Damage(damage);
    }

    public void Damage(float damage, Vector3 force)
    {
        damageable.Damage(damage, force);
    }
}
