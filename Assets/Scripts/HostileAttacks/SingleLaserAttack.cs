using System.Collections;
using UnityEngine;

public class SingleLaserAttack : LaserAttacker
{
    protected override IEnumerator Attack(Transform target)
    {
        BossBarrel[] selectedSet = rotatingBarrelSets[Random.Range(0, rotatingBarrelSets.Length)];
        BossBarrel selected = selectedSet[Random.Range(0, selectedSet.Length)];

        yield return StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
    }
}
