using System;
using UnityEngine.InputSystem;

public abstract class DestroyTriggerOnActivate : TextPromptKeyTrigger
{
    protected Action onActivate;

    protected override void CallActivate(InputAction.CallbackContext ctx)
    {
        onActivate?.Invoke();
        base.CallActivate(ctx);
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
        if (gameObject.name != "Door")
        {
            Destroy(gameObject);
        }
    }
}
