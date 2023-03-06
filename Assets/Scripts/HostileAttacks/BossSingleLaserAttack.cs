using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSingleLaserAttack : BossLaserAttack
{
    [SerializeField] private Transform[] barrelOptionHolders;
    private List<LaserBarrel> options = new List<LaserBarrel>();

    private new void Awake()
    {
        // Fetch and add all options
        foreach (Transform holder in barrelOptionHolders)
        {
            options.AddRange(holder.GetComponentsInChildren<LaserBarrel>());
        }
        base.Awake();
    }

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        LaserBarrel selected = options[Random.Range(0, options.Count)];
        StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
        yield return new WaitUntil(() => selected.IsFiring);
        yield return new WaitUntil(() => !selected.IsFiring);
    }
}
