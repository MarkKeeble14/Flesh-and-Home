using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShockwaveAttack : Attacker
{
    [SerializeField] private ShockwaveRing ring;
    [SerializeField] private int numThumps;
    [SerializeField] private float timeBetweenThumps;
    [SerializeField] private float maxRadius;
    [SerializeField] private float height;
    [SerializeField] private float spawnAtYOffset;
    [SerializeField] private float expansionSpeed;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask canHit;

    [SerializeField] private ImpulseSourceData impulseSourceData;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer emitClip;
    [SerializeField] private AudioClipContainer endClip;

    protected override IEnumerator Attack(Transform target)
    {
        List<ShockwaveRing> spawnedRings = new List<ShockwaveRing>();

        for (int i = 0; i < numThumps; i++)
        {
            // Audio
            emitClip.PlayOneShot(source);

            ShockwaveRing spawned = Instantiate(ring, transform.position + Vector3.up * spawnAtYOffset, Quaternion.identity);
            spawnedRings.Add(spawned);

            StartCoroutine(spawned.ExecuteThump(() =>
            {
                spawnedRings.Remove(spawned);

                Destroy(spawned);
                // Debug.Log("Removed: " + spawned + ", Remaining: " + spawnedRings.Count);
            }, maxRadius, height, expansionSpeed, damage, canHit, impulseSourceData));

            if (i != numThumps - 1)
                yield return new WaitForSeconds(timeBetweenThumps);
        }

        yield return new WaitUntil(() => spawnedRings.Count == 0);

        // Audio
        endClip.PlayOneShot(source);
    }
}
