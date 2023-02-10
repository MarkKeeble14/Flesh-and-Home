using UnityEngine;
using TMPro;

public class FuelStoreDisplay : MonoBehaviour
{
    [SerializeField] private ImageSliderBar sliderBar;
    [SerializeField] private FuelStore fuel;

    private void Update()
    {
        sliderBar.Set(fuel.CurrentFuel, fuel.StartingFuel);
    }
}

