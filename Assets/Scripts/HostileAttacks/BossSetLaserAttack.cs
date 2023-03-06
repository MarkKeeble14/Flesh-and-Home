using System.Collections;
using UnityEngine;

public class BossSetLaserAttack : BossLaserAttack
{
    [SerializeField] private Transform barrelSetHolder;
    private LaserBarrel[] set;

    private new void Awake()
    {
        // Get Barrels
        set = barrelSetHolder.GetComponentsInChildren<LaserBarrel>();
        base.Awake();
    }

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
