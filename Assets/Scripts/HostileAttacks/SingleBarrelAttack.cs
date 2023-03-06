using System.Collections;
using UnityEngine;

public class SingleBarrelAttack : StandardLaserAttack
{
    [SerializeField] private LaserBarrel barrel;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        StartCoroutine(LaserFrom(barrel, target));

        yield return new WaitUntil(() => barrel.IsFiring);

        yield return new WaitUntil(() => !barrel.IsFiring);
    }
}
