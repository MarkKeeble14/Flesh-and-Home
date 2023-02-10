using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    // Make a singleton for easy access + also there should obviously only ever be one crosshair
    public static CrosshairController _Instance;
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance);
        }
        _Instance = this;
    }

    [SerializeField] private CrosshairMemberController[] crosshairMembers;

    public void Spread(float force)
    {
        foreach (CrosshairMemberController member in crosshairMembers)
        {
            member.Spread(force);
        }
    }

    public void Release()
    {
        foreach (CrosshairMemberController member in crosshairMembers)
        {
            member.Release();
        }
    }

    private void Update()
    {
        Release();
    }
}
