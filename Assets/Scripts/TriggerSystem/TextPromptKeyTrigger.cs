using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TextPromptKeyTrigger : EventManager
{
    [SerializeField] protected string activePrompt;

    protected TriggerHelperText helperText;

    public override bool ActivateOnCollide { get { return false; } }

    private string prefix = "'E' to ";
    public virtual string Prefix
    {
        get
        {
            return prefix;
        }
        set
        {
            prefix = value;
        }
    }

    protected void Start()
    {
        // Find Helper Text
        helperText = FindObjectOfType<TriggerHelperText>();
    }

    protected void ShowText()
    {
        foreach (GameEvent gameEvent in gameEvents)
        {
            if (!gameEvent.UseText) continue;
            helperText.Show(gameEvent, GetHelperTextString(gameEvent));
        }
    }

    protected void HideText()
    {
        foreach (GameEvent gameEvent in gameEvents)
        {
            if (!gameEvent.UseText) continue;
            helperText.Hide(gameEvent);
        }
    }

    protected override void CallActivate(InputAction.CallbackContext ctx)
    {
        if (destroyOnActivate)
        {
            HideText();
            InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
        }
        base.CallActivate(ctx);
    }

    protected override void OnEnter(Collider other)
    {
        ShowText();

        // Add Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;
    }

    protected override void OnExit(Collider other)
    {
        HideText();

        //
        // Remove Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
    }

    protected string GetHelperTextString(GameEvent gameEvent)
    {
        return Prefix + gameEvent.Label + gameEvent.EventString;
    }
}