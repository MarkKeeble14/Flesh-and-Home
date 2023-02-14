using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "LaserVisuals/Gradient", fileName = "GradientLaserVisuals")]
public class GradientLaserVisuals : LaserVisuals
{
    [SerializeField] private Gradient gradient;

    public override Color GetDefaultColor()
    {
        return gradient.colorKeys[0].color;
    }

    public override Color GetLerpedColor(float lerpBy)
    {
        return gradient.Evaluate(lerpBy);
    }

    public override Color GetMaxColor()
    {
        return gradient.colorKeys[gradient.colorKeys.Length - 1].color;
    }

    public override void SetDefaultColor(Color color)
    {
        List<GradientColorKey> keys = gradient.colorKeys.ToList();

        keys[0] = new GradientColorKey(color, gradient.colorKeys[0].time);

        gradient.SetKeys(keys.ToArray(), gradient.alphaKeys);
    }
}
