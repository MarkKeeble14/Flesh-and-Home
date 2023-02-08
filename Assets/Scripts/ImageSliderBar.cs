using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSliderBar : MonoBehaviour
{
    [SerializeField] private bool showWhenFull;
    [SerializeField] private bool subtractMode;
    [SerializeField] private bool useText;
    [SerializeField] private bool valueAsText;
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;
    [SerializeField] private int decimalPlaces = 1;
    [SerializeField] private HorizontalDirection direction;

    private float value;
    private float maxValue;

    [Header("References")]
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        // Set Value and Control Object State
        float currentValue = value / maxValue;
        bool shouldBeActive = (currentValue > 0 && value < maxValue) || (currentValue > 0 && showWhenFull);
        // Debug.Log(value + ", " + maxValue + ", " + currentValue + ", " + shouldBeActive);

        // If not set to use text, don't use text
        text.gameObject.SetActive(shouldBeActive && useText);

        // Don't show the bar when either at 0 or equal to or above 1
        bar.gameObject.SetActive(shouldBeActive);

        // Set the bar fill amount and text value
        if (subtractMode)
        {
            bar.fillAmount = 1 - currentValue;
        }
        else
        {
            bar.fillAmount = currentValue;
        }
        bar.fillOrigin = direction == HorizontalDirection.RIGHT ? 1 : 0;

        text.text =
            prefix +
            (valueAsText ?
                value + " / " + maxValue :
                (System.Math.Round(maxValue - (currentValue * maxValue), decimalPlaces)).ToString())
            + suffix;
    }

    public void SetValue(float v)
    {
        value = v;
    }

    public void SetMaxValue(float v)
    {
        maxValue = v;
    }

    public void Set(float current, float max)
    {
        value = current;
        maxValue = max;
    }

    public void SetColor(Color c)
    {
        bar.color = c;
    }

    public void SetHorizontalDirection(HorizontalDirection direction)
    {
        this.direction = direction;
    }
}
