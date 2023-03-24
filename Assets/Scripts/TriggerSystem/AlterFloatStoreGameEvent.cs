using UnityEngine;

public class AlterFloatStoreGameEvent : GameEvent
{
    [SerializeField] private FloatStore fuelStore;
    [SerializeField] private float fuelAmount;

    protected override void Activate()
    {
        fuelStore.AlterFloat(fuelAmount);
    }
}
