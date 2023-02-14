using System.Collections;
using UnityEngine;

public class SetBarrelAttack : StandardLaserAttack
{
    [SerializeField] private LaserBarrel[] set;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        foreach (LaserBarrel barrel in set)
        {
            StartCoroutine(LaserFrom(barrel, target));
        }

        yield return new WaitUntil(() => GetLasersActive(set));

        yield return new WaitUntil(() => !GetLasersActive(set));
    }
}
