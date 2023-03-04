using System.Collections;
using UnityEngine;

public class BossDefinedBarrelsLaserAttack : BossLaserAttack
{
    private LaserBarrel[] set;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        for (int i = 0; i < set.Length; i++)
        {
            LaserBarrel selected = set[i];
            StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
        }

        yield return new WaitUntil(() => GetLasersActive(set));

        while (GetLasersActive(set) && !executionInterrupted)
        {
            yield return null;
        }
    }
}