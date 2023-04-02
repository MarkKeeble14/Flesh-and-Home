using System.Collections;
using UnityEngine;

public class ProjectileAttack : Attack
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform projectileOrigin;
    [SerializeField] private LayerMask canHit;
    [SerializeField] private LayerMask canDamage;
    [SerializeField] private float damage, speed, force;

    [SerializeField] private Animator anim;
    [SerializeField] private string attackAnimatorParameter;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        Projectile shot = Instantiate(projectile, projectileOrigin.position, Quaternion.identity);
        shot.SetTarget(target.position, canHit, canDamage, speed, damage, force);

        if (anim != null)
            anim.SetTrigger(attackAnimatorParameter);

        yield return null;
    }
}
