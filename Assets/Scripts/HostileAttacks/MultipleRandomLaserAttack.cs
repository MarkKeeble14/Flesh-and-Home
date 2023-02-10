using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleRandomLaserAttack : LaserAttacker
{
    [Header("Specific Settings")]
    [SerializeField] private int maxLaserCalls = 8;

    protected override IEnumerator Attack(Transform target)
    {
        int toCall = Random.Range(1, maxLaserCalls);
        List<BossBarrel> callOn = new List<BossBarrel>();

        // Picking a random number of random barrels
        // Should potentially be a while loop so as to guarentee the maximum number of lasers being called; but dealing with the overlap of finding ones
        // which are unused seems not worthwhile for now at least
        for (int i = 0; i < toCall; i++)
        {
            BossBarrel[] selectedSet = rotatingBarrelSets[Random.Range(0, rotatingBarrelSets.Length)];
            BossBarrel selected = selectedSet[Random.Range(0, selectedSet.Length)];

            // if a barrel has already been selected, ignore it
            if (!callOn.Contains(selected))
            {
                callOn.Add(selected);
            }
        }

        for (int i = 0; i < callOn.Count; i++)
        {
            BossBarrel selected = callOn[i];
            StartCoroutine(LaserFrom(selected, bossPhaseManager.ShellEnemyMovement.transform));
        }

        yield return new WaitUntil(() => GetLasersActive(callOn));

        yield return new WaitUntil(() => !GetLasersActive(callOn));
    }
}