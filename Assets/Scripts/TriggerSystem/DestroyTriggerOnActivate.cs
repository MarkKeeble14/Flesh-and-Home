using System;
using UnityEngine.InputSystem;

public abstract class DestroyTriggerOnActivate : EventTrigger
{
    protected void Awake()
    {
        onActivate += () => Destroy(gameObject);
    }
}
