using UnityEngine;

[CreateAssetMenu(fileName = "FuelStore", menuName = "FuelStore")]
public class FuelStore : ScriptableObject
{
    // Fuel
    [Range(0, 100)] [SerializeField] private float startingFuel = 100;
    public float StartingFuel => startingFuel;
    [Range(0, 100)] [SerializeField] private float currentFuel;
    public float CurrentFuel
    {
        get
        {
            return currentFuel;
        }
    }

    public void AlterFuel(float amount)
    {
        if (currentFuel + amount < 0)
        {
            currentFuel = 0;
            return;
        }
        if (currentFuel + amount > 100)
        {
            currentFuel = 100;
            return;
        }
        currentFuel += amount;
    }

    public void Reset()
    {
        // Set Fuel
        currentFuel = startingFuel;
    }
}

