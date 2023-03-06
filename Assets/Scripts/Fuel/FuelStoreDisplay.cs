using UnityEngine;
using TMPro;

public class FuelStoreDisplay : MonoBehaviour
{
    [SerializeField] private ImageSliderBar sliderBar;
    [SerializeField] private FloatStore fuel;

    private void Update()
    {
        sliderBar.Set(fuel.CurrentFloat, fuel.StartingFloat);
    }
}

