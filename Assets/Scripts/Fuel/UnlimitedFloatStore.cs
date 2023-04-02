using UnityEngine;

[CreateAssetMenu(fileName = "UnlimitedFloatStore", menuName = "UnlimitedFloatStore")]
public class UnlimitedFloatStore : ScriptableObject
{
    // Fuel
    [SerializeField] private float startingFloat = 100;
    public float StartingFloat => startingFloat;
    [SerializeField] private float currentFloat;
    public float CurrentFloat
    {
        get
        {
            return currentFloat;
        }
        set
        {
            currentFloat = value;
        }
    }

    public void AlterFloat(float amount)
    {
        currentFloat += amount;
    }

    public void Reset()
    {
        // Set Fuel
        // Debug.Log("To Reset: " + currentFloat + ", " + startingFloat);
        currentFloat = startingFloat;
        // Debug.Log("Reset: " + currentFloat);
    }
}