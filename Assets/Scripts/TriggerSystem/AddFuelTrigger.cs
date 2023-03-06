using UnityEngine;
using UnityEngine.InputSystem;

public class AddFuelTrigger : TextPromptKeyTrigger
{
    [SerializeField] private FloatStore fuelStore;
    [SerializeField] private float fuelAmount;

    private new void Awake()
    {
        onActivate += () => fuelStore.AlterFloat(fuelAmount);
        base.Awake();
    }
}

