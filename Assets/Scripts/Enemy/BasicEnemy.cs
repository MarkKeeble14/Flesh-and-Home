using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private List<Attack> attackPool = new List<Attack>();

    protected Attack nextAttack;
    protected List<Attack> currentAttacks = new List<Attack>();

    public bool AttackIsDisablingMove
    {
        get
        {
            foreach (Attack attack in currentAttacks)
            {
                if (attack.IsDisablingMovement)
                {
                    // Debug.Log("Attack: " + attack.IsDisablingMovement);
                    return true;
                }
            }
            return false;
        }
    }

    private void Start()
    {
        SetNextAttack(false);
    }

    protected IEnumerator StartNextAttack(Transform target)
    {
        SetNextAttack(true);
        yield return StartCoroutine(StartAttack(target));
    }

    protected IEnumerator StartAttack(Transform target)
    {
        yield return StartCoroutine(nextAttack.StartAttack(target, this));

        // Add back to attack pool if desired
        if (nextAttack.RemoveFromPoolWhenActive)
        {
            attackPool.Add(nextAttack);
        }
    }

    private void SetNextAttack(bool canRemoveFromPool)
    {
        if (attackPool.Count > 0)
        {
            nextAttack = attackPool[Random.Range(0, attackPool.Count)];

            // Remove from attack pool if desired
            if (canRemoveFromPool && nextAttack.RemoveFromPoolWhenActive)
            {
                attackPool.Remove(nextAttack);
            }
        }
    }

    public void AddAttack(Attack attack)
    {
        currentAttacks.Add(attack);
    }

    public void RemoveAttack(Attack attack)
    {
        // Contains a duplicate, remove the first instance
        if (currentAttacks.Exists(a => a == attack))
        {
            for (int i = 0; i < currentAttacks.Count; i++)
            {
                if (currentAttacks[i] == attack) currentAttacks.RemoveAt(i);
            }
        }
        else // no duplicates, remove only instance
        {
            currentAttacks.Remove(attack);
        }
    }
}
