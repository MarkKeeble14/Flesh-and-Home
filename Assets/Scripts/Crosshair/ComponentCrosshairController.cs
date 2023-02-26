using System.Collections;
using UnityEngine;

public class ComponentCrosshairController : CrosshairController
{
    [SerializeField] private CrosshairComponent[] crosshairMembers;

    public override void Spread(float force)
    {
        foreach (CrosshairComponent member in crosshairMembers)
        {
            member.Spread(force, spreadSpeed);
        }
    }

    public override void Release()
    {
        foreach (CrosshairComponent member in crosshairMembers)
        {
            member.Release(releaseSpeed);
        }
    }
}
