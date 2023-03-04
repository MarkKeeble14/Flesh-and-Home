using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackingEnemy : MonoBehaviour
{
    [SerializeField] private AttackPool attackPool = new AttackPool();
    protected SerializableKeyValuePair<Attack, int> attack;
    protected List<Attack> currentAttacks = new List<Attack>();
    public bool IsDead { get; private set; }

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

    public Attack SetAttack()
    {
        attack = attackPool.GetAttack();
        return attack.Key;
    }

    public IEnumerator StartAttack(Transform target, bool setNewAttack)
    {
        if (!attackPool.HasAttacksAvailable) yield break;

        SerializableKeyValuePair<Attack, int> attack = this.attack;
        if (setNewAttack)
            attack = attackPool.GetAttack();

        // Remove from attack pool if desired
        if (attack.Key.RemoveFromPoolWhenActive)
        {
            attackPool.RemoveAttack(attack);
        }
        yield return StartCoroutine(attack.Key.StartAttack(target, this));

        // Add back to attack pool if desired
        if (attack.Key.RemoveFromPoolWhenActive)
        {
            attackPool.AddAttack(attack);
        }
    }

    public void AddCurrentAttack(Attack attack)
    {
        currentAttacks.Add(attack);
    }

    public void RemoveCurrentAttack(Attack attack)
    {
        currentAttacks.Remove(attack);
    }

    public void Stop()
    {
        // Stop all attacks
        foreach (Attack attack in currentAttacks)
        {
            attack.Interrupt();
        }
    }

    public void NotifyOfDeath()
    {
        // Bad?
        IsDead = true;
        Stop();
    }
}
