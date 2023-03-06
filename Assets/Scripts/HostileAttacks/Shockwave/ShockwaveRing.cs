using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cinemachine;

public class ShockwaveRing : MonoBehaviour
{
    [SerializeField] private List<Collider> hasCollidedWith = new List<Collider>();
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private float minAlpha = .1f;
    [SerializeField] private new Collider collider;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClipContainer hitClip;

    private LayerMask canHit;
    private float damage;
    private ImpulseSourceData impulseSourceData;
    [SerializeField] private CinemachineImpulseSource collideWithPlayerImpulseSource;

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, canHit)) return;

        if (hasCollidedWith.Contains(other)) return;

        hasCollidedWith.Add(other);

        // Audio
        hitClip.PlayOneShot(source);

        if (other.TryGetComponent(out IDamageable damageable))
        {
            collideWithPlayerImpulseSource.GenerateImpulse(
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

    public void FadeOut()
    {
        StartCoroutine(ExecuteFadeOut());
    }

    private IEnumerator ExecuteFadeOut()
    {
        collider.enabled = false;

        Material material = renderer.material;
        Color color = material.color;
        Color transparentColor = color;
        transparentColor.a = 0;

        material.DisableKeyword("_EMISSION");

        float t = 0;
        while (color.a > minAlpha)
        {
            color = material.color;
            transparentColor = color;
            transparentColor.a = 0;

            // Debug.Log("Current Color: " + color + ", Transparent Color: " + transparentColor + ", Current Alpha: " + color.a);
            material.color = Color.Lerp(color, transparentColor, t * fadeOutSpeed);

            t += Time.deltaTime;
            yield return null;
        }

        // Debug.Log("Destroying");
        Destroy(gameObject);
    }
}
