using UnityEngine;

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
