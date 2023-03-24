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

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, canHit)) return;

        if (LayerMaskHelper.IsInLayerMask(other.gameObject, canDamage))
        {
            // Try to do damage
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage, Vector3.zero, DamageSource.RIFLE);
            }
        }

        Explode();
    }

    private IEnumerator Travel(Vector3 target, float speed)
    {
        float step = speed * Time.deltaTime;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            yield return null;
        }
        Debug.Log(name + ", Destroying After Reach Target");
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

    void Explode()
    {
        // Spawn Particles
        // Explosion
        PlayerLaserExplosiveParticleSystem explosion = Instantiate(explosionParticleSystem, transform.position, Quaternion.identity);
        explosion.SetColors(projectileColor, projectileColor, projectileColor);

        Destroy(gameObject);
    }
}