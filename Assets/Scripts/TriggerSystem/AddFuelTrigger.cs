using UnityEngine;
using UnityEngine.InputSystem;

public class AddFuelTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private FloatStore fuelStore;
    [SerializeField] private float fuelAmount;

    protected override void Activate()
    {
        fuelStore.AlterFloat(fuelAmount);
    }
}

