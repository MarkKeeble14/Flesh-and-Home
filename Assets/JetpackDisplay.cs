using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetpackDisplay : MonoBehaviour
{
    [SerializeField] private ImageSliderBar bar;

    public void Set(float current, float max)
    {
        bar.Set(current, max);
    }
}
