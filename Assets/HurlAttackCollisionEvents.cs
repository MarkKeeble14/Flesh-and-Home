using System.Collections;
using UnityEngine;

public class HurlAttackCollisionEvents : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private AudioClipContainer onExplode;
    [SerializeField] private TemporaryAudioSource tempAudioSource;

    [SerializeField] private GameObject[] particlesOnExplode;

    [SerializeField] private LayerMask groundLayer;

    public bool primed;

    private void OnTriggerEnter(Collider other)
    {
        if (!primed) return;

        if (LayerMaskHelper.IsInLayerMask(other.gameObject, groundLayer))
        {
            StartCoroutine(ExplodeAfterTime());
        }
        else
        {
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage, DamageSource.ENEMY_SWARM);
            }
        }
    }

    private void Explode()
    {

        Instantiate(tempAudioSource, transform.position, Quaternion.identity).Play(onExplode);

        SpawnParticles();

        KillableEntity k = GetComponentInParent<KillableEntity>();
        k.AcceptDamage = true;
        k.Damage(k.CurrentHealth, DamageSource.ENEMY_SWARM);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(5f);

        Explode();
    }

    private void SpawnParticles()
    {
        foreach (GameObject obj in particlesOnExplode)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }
    }
}
