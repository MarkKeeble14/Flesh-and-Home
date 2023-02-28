using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
    [SerializeField] private new Light light;
    [SerializeField] private PlayerLaserExplosiveParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem impactEffect;
    private LayerMask canHit, canDamage;
    private float damage, impactForce;
    private Color projectileColor;

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, canHit)) return;

        if (LayerMaskHelper.IsInLayerMask(collision.gameObject, canDamage))
        {
            // Try to do damage
            if (LayerMaskHelper.IsInLayerMask(collision.gameObject, canHit))
            {
                if (collision.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(damage, -collision.GetContact(0).normal * impactForce);
                }
            }
        }

        Explode(collision);
    }

    private IEnumerator Travel(Vector3 target, float speed)
    {
        float step = speed * Time.deltaTime;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target, Color projectileColor, LayerMask canHit, LayerMask canDamage, float speed, float damage, float force)
    {
        this.canHit = canHit;
        this.canDamage = canDamage;
        this.projectileColor = projectileColor;
        this.damage = damage;
        this.impactForce = force;
        light.color = projectileColor;
        StartCoroutine(Travel(target, speed));
    }

    void Explode(Collision collision)
    {
        ContactPoint point = collision.GetContact(0);

        // Spawn Particles
        // Explosion
        PlayerLaserExplosiveParticleSystem explosion = Instantiate(explosionParticleSystem, point.point, Quaternion.identity);
        explosion.SetColors(projectileColor, projectileColor, projectileColor);

        // Metal
        // Eventually could do a dictionary with different layers to pick out different particles (so hitting different surfaces would produce different particles
        Instantiate(impactEffect, point.point, Quaternion.LookRotation(point.normal));

        Destroy(gameObject);
    }
}