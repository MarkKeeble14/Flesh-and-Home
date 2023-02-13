using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLaserAttack : LaserAttacker
{
    [SerializeField] private Transform[] barrelOptionHolders;
    private List<BossBarrel> options = new List<BossBarrel>();

    private new void Awake()
    {
        // Fetch and add all options
        foreach (Transform holder in barrelOptionHolders)
        {
            options.AddRange(holder.GetComponentsInChildren<BossBarrel>());
        }
        base.Awake();
    }

    protected override IEnumerator Attack(Transform target)
    {
        BossBarrel selected = options[Random.Range(0, options.Count)];
        yield return StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
    }
}
