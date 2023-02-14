using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShockwaveRing : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClipContainer hitClip;

    private LayerMask canHit;
    private float damage;

    [SerializeField] private List<Collider> hasCollidedWith = new List<Collider>();

    private ImpulseSourceData impulseSourceData;

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, canHit)) return;

        if (hasCollidedWith.Contains(other)) return;

        hasCollidedWith.Add(other);

        // Audio
        hitClip.PlayOneShot(source);

        if (other.TryGetComponent(out IDamageable damageable))
        {
            impulseSourceData.collideWithPlayerImpulseSource.GenerateImpulse(
                new Vector3(
                    RandomHelper.RandomFloat(impulseSourceData.horizontalMinMaxImpulse),
                    RandomHelper.RandomFloat(impulseSourceData.verticalMinMaxImpulse),
                    0) * impulseSourceData.impulseMultiplier
                );
            damageable.Damage(damage);
        }
    }

    public IEnumerator ExecuteThump(Action onEnd, float maxRadius, float height, float expansionSpeed, float damage, LayerMask canHit, ImpulseSourceData impulseSourceData)
    {
        this.damage = damage;
        this.canHit = canHit;
        this.impulseSourceData = impulseSourceData;
        float currentRadius = 0f;

        // Expand
        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * expansionSpeed;
            transform.localScale = new Vector3(currentRadius, height, currentRadius);
            yield return null;
        }

        onEnd();
    }
}
