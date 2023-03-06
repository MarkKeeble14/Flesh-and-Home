using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShockwaveAttack : Attack
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
    private LayerMask ground;

    [SerializeField] private ImpulseSourceData impulseSourceData;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer emitClip;
    [SerializeField] private AudioClipContainer endClip;

    private List<ShockwaveRing> spawnedRings;

    private void Awake()
    {
        ground = LayerMask.GetMask("Ground");
    }

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        spawnedRings = new List<ShockwaveRing>();

        for (int i = 0; i < numThumps; i++)
        {
            // Audio
            emitClip.PlayOneShot(source);

            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, ground);

            ShockwaveRing spawned = Instantiate(ring, hit.point + Vector3.up * spawnAtYOffset, Quaternion.identity);
            spawnedRings.Add(spawned);

            spawned.StartCoroutine(spawned.ExecuteThump(() =>
            {
                spawnedRings.Remove(spawned);
                spawned.FadeOut();
                // Debug.Log("Removed: " + spawned + ", Remaining: " + spawnedRings.Count);
            }, maxRadius, height, expansionSpeed, damage, canHit, impulseSourceData));

            if (executionInterrupted)
                break;

            if (i != numThumps - 1)
                yield return new WaitForSeconds(timeBetweenThumps);
        }

        if (!executionInterrupted)
        {
            yield return new WaitUntil(() => spawnedRings.Count == 0);

            // Audio
            endClip.PlayOneShot(source);
        }
    }
}
