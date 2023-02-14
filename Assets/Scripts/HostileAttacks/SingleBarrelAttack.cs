using System.Collections;
using UnityEngine;

public class SingleBarrelAttack : StandardLaserAttack
{
    [SerializeField] private LaserBarrel barrel;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        yield return StartCoroutine(LaserFrom(barrel, target));
    }
}
