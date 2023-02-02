using UnityEngine;
using TMPro;

public class FuelStoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private FuelStore fuel;

    private void Update()
    {
        // Update Text
        text.text = "Fuel: " + System.Math.Round(fuel.CurrentFuel, 1) + "/" + fuel.StartingFuel;
    }
}

