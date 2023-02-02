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
        set
        {
            currentFuel = value;
        }
    }

    public void Reset()
    {
        // Set Fuel
        currentFuel = startingFuel;
    }
}

