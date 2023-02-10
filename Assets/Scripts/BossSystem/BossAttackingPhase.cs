using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossAttackingPhase : BossPhaseBaseState
{
    [Header("Attacks")]
    [SerializeField] private List<Attacker> attacks = new List<Attacker>();
    [SerializeField] protected int numAttacksAtOnce = 1;
    [SerializeField] private float timeBetweenAttacks;
    private List<Attacker> currentAttacks = new List<Attacker>();

    private bool CurrentlyAttacking
    {
        get
        {
            foreach (Attacker attack in currentAttacks)
            {
                if (attack.CurrentlyAttacking) return true;
            }
            return false;
        }
    }

    private Attacker GetAttack()
    {
        if (attacks.Count == 0) return null;
        return attacks[Random.Range(0, attacks.Count)];
    }

    protected IEnumerator CallAttacks(BossPhaseManager boss)
    {
        for (int i = 0; i < numAttacksAtOnce; i++)
        {
            // try to grab an attack
            Attacker attack = GetAttack();

            if (attack == null)
            {
                // Debug.Log("Attempted to Call More Attacks than are Available");
                break;
            }

            // Debug.Log("Starting Attack: " + attack + ", Attack #: " + i);

            attacks.Remove(attack);
            // Debug.Log("Removed: " + attack + ", Num Attacks Available: " + attacks.Count);
            currentAttacks.Add(attack);

            // Call Attack
            StartCoroutine(attack.StartAttack(GameManager._Instance.PlayerTransform,
                delegate
                {
                    attacks.Add(attack);
                    currentAttacks.Remove(attack);
                    // Debug.Log("Re-Added: " + attack + ", Num Attacks Available: " + attacks.Count);
                }));
        }

        // Debug.Log("Waiting to be Attacking");

        yield return new WaitUntil(() => CurrentlyAttacking);

        // Debug.Log("Started Attacking: Waiting to Stop Attacking");

        yield return new WaitUntil(() => !CurrentlyAttacking);

        // Debug.Log("No Longer Attacking");

        yield return new WaitForSeconds(timeBetweenAttacks);
    }
}
