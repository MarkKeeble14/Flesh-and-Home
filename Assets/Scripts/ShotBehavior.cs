using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private new Light light;
    [SerializeField] private PlayerLaserExplosiveParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem impactEffect;
    private Rifle raygun;
    private float damage, impactForce;
    [SerializeField] private LayerMask canDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, raygun.CanHit)) return;

        // Try to do damage
        if (LayerMaskHelper.IsInLayerMask(collision.gameObject, canDamage))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage, -collision.GetContact(0).normal * impactForce);
            }
        }

        Explode(collision);
    }

    private IEnumerator Travel(Vector3 target)
    {
        float step = speed * Time.deltaTime;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void setTarget(Vector3 target, Rifle raygun, float damage, float force)
    {
        this.raygun = raygun;
        light.color = raygun.CurrentColor;
        this.damage = damage;
        this.impactForce = force;
        StartCoroutine(Travel(target));
    }

    void Explode(Collision collision)
    {
        ContactPoint point = collision.GetContact(0);

        // Spawn Particles
        // Explosion
        PlayerLaserExplosiveParticleSystem explosion = Instantiate(explosionParticleSystem, point.point, Quaternion.identity);
        explosion.SetColors(raygun.CurrentColor, raygun.CurrentColor, raygun.CurrentColor);

        // Metal
        // Eventually could do a dictionary with different layers to pick out different particles (so hitting different surfaces would produce different particles
        Instantiate(impactEffect, point.point, Quaternion.LookRotation(point.normal));

        Destroy(gameObject);
    }
}