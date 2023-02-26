using UnityEngine;
using UnityEngine.InputSystem;

public class AddFuelTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private FuelStore fuelStore;
    [SerializeField] private float fuelAmount;

    protected override void Activate()
    {
        fuelStore.AlterFuel(fuelAmount);
    }
}

