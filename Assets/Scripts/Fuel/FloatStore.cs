using UnityEngine;

[CreateAssetMenu(fileName = "FloatStore", menuName = "FloatStore")]
public class FloatStore : ScriptableObject
{
    // Fuel
    [Range(0, 100)] [SerializeField] private float startingFloat = 100;
    public float StartingFloat => startingFloat;
    [Range(0, 100)] [SerializeField] private float currentFloat;
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
    public float MaxFloat
    {
        get
        {
            return startingFloat;
        }
    }

    public void AlterFloat(float amount)
    {
        if (currentFloat + amount < 0)
        {
            currentFloat = 0;
            return;
        }
        if (currentFloat + amount > 100)
        {
            currentFloat = 100;
            return;
        }
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
