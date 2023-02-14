using System;
using UnityEngine.InputSystem;

public abstract class DestroyTriggerOnActivate : TextPromptKeyTrigger
{
    protected Action onActivate;

    protected override void CallActivate(InputAction.CallbackContext ctx)
    {
        base.CallActivate(ctx);
        onActivate();
    }

    public void AddOnActivate(Action action)
    {
        onActivate += action;
    }

    protected void Awake()
    {
        onActivate += Destroy;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
