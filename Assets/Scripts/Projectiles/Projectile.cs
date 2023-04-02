using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnOnHit;
    private LayerMask canHit, canDamage;
    private float damage, impactForce;

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
        // Debug.Log(name + ", Destroying After Reach Target");
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target, LayerMask canHit, LayerMask canDamage, float speed, float damage, float force)
    {
        this.canHit = canHit;
        this.canDamage = canDamage;
        this.damage = damage;
        this.impactForce = force;
        StartCoroutine(Travel(target, speed));
    }

    void Explode()
    {
        // Spawn Particles
        // Explosion
        foreach (GameObject obj in spawnOnHit)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
