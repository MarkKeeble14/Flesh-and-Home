using UnityEngine;

[CreateAssetMenu(menuName = "LaserVisuals/StaticColored", fileName = "StaticColoredLaserVisuals")]
public class StaticColoredLaserVisuals : LaserVisuals
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color maxColor;

    public override Color GetDefaultColor()
    {
        return defaultColor;
    }

    public override Color GetLerpedColor(float lerpBy)
    {
        return Color.Lerp(defaultColor, maxColor, lerpBy);
    }

    public override Color GetMaxColor()
    {
        return maxColor;
    }

    public override void SetDefaultColor(Color color)
    {
        defaultColor = color;
    }
}
