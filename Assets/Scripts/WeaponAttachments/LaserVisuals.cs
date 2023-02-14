using System;
using UnityEngine;

public abstract class LaserVisuals : ScriptableObject
{
    [SerializeField] private Vector2 emissionIntensityScale;

    public Color GetEmmissiveColor(Color color)
    {
        return color * emissionIntensityScale.y;
    }

    public Color GetEmmissiveColor(Color color, float percent)
    {
        return color * (emissionIntensityScale.x + (emissionIntensityScale.y * percent));
    }

    public abstract Color GetLerpedColor(float lerpBy);

    public abstract Color GetDefaultColor();

    public abstract Color GetMaxColor();

    public abstract void SetDefaultColor(Color color);
}
