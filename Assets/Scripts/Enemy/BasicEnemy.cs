using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AttackPool
{
    [SerializeField] private PercentageMap<Attack> available = new PercentageMap<Attack>();
    private PercentageMap<Attack> reserve = new PercentageMap<Attack>();

    public bool HasAttacksAvailable => NumAvailableAttacks > 0;
    public int NumAvailableAttacks => available.Count;

    public SerializableKeyValuePair<Attack, int> GetAttack()
    {
        return available.GetFullOption();
    }

    public void RemoveAttack(SerializableKeyValuePair<Attack, int> attack)
    {
        available.RemoveOption(attack);
        reserve.AddOption(attack);
    }

    public void AddAttack(SerializableKeyValuePair<Attack, int> attack)
    {
        reserve.RemoveOption(attack);
        available.AddOption(attack);
    }
}

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private AttackPool attackPool = new AttackPool();
    protected SerializableKeyValuePair<Attack, int> attack;
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

    protected void SetAttack()
    {
        attack = attackPool.GetAttack();
    }

    protected IEnumerator StartAttack(Transform target, bool setNewAttack)
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

    public void AddAttack(Attack attack)
    {
        currentAttacks.Add(attack);
    }

    public void RemoveAttack(Attack attack)
    {
        currentAttacks.Remove(attack);
        /*
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
        */
    }
}
