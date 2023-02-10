using System.Collections;
using UnityEngine;

public class SetLaserAttack : LaserAttacker
{
    [SerializeField] private Transform barrelSetHolder;
    private BossBarrel[] set;

    private new void Awake()
    {
        // Get Barrels
        set = barrelSetHolder.GetComponentsInChildren<BossBarrel>();
        base.Awake();
    }

    protected override IEnumerator Attack(Transform target)
    {
        for (int i = 0; i < set.Length; i++)
        {
            BossBarrel selected = set[i];
            StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
        }

        yield return new WaitUntil(() => GetLasersActive(set));

        yield return new WaitUntil(() => !GetLasersActive(set));
    }
}
