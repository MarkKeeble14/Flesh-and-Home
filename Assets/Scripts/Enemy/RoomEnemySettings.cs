using UnityEngine;

[CreateAssetMenu(fileName = "RoomEnemySettings", menuName = "RoomEnemySettings")]
public class RoomEnemySettings : ScriptableObject
{
    public Color inactiveColor;
    public Color activeColor;
    public float colorIntensity;

    public void SetInactiveColors(Renderer renderer)
    {
        renderer.material.color = inactiveColor;
        renderer.material.DisableKeyword("_EMISSION");
    }

    public void SetActiveColors(Renderer renderer)
    {
        renderer.material.color = activeColor;
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", activeColor * colorIntensity);
    }
}
