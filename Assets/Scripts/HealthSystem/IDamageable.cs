using System;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damage, DamageSource source);
    void Damage(float damage, Vector3 force, DamageSource source);
}
