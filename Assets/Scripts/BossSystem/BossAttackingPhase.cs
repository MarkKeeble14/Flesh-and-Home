using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossAttackingPhase : BossPhaseBaseState
{
    [Header("Attacks")]
    [SerializeField] protected int numAttacksAtOnce = 1;
    [SerializeField] private float timeBetweenAttacks;

    protected IEnumerator CallAttacks(BossPhaseManager boss, Transform target)
    {
        for (int i = 0; i < numAttacksAtOnce; i++)
        {
            StartCoroutine(StartNextAttack(target));
        }

        // Debug.Log("Waiting to be Attacking");

        yield return new WaitUntil(() => currentAttacks.Count > 0);

        // Debug.Log("Started Attacking: Waiting to Stop Attacking");

        yield return new WaitUntil(() => currentAttacks.Count == 0);

        // Debug.Log("No Longer Attacking");

        yield return new WaitForSeconds(timeBetweenAttacks);
    }
}
