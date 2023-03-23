using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class TextPromptKeyTrigger : EventTrigger
{
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
    private string suffix = "";
    protected virtual string Suffix
    {
        get
        {
            return suffix;
        }
        set
        {
            suffix = value;
        }
    }
    [SerializeField] protected string activePrompt;

    protected TriggerHelperText helperText;
    protected bool showText = true;

    protected virtual bool AllowShowText
    {
        get
        {
            return true;
        }
    }

    [SerializeField] private bool destroyOnActivate = true;

    protected new void Awake()
    {
        if (destroyOnActivate)
        {

            onActivate += () => Destroy(gameObject);
        }

        base.Awake();
    }

    protected void Start()
    {
        // Find Helper Text
        helperText = FindObjectOfType<TriggerHelperText>();
    }

    protected override void OnEnter(Collider other)
    {
        if (showText && AllowShowText)
        {
            helperText.Show(this, GetHelperTextString());
        }

        // Add Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;
    }

    protected override void OnExit(Collider other)
    {
        helperText.Hide(this);

        //
        // Remove Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
    }

    protected string GetHelperTextString()
    {
        return Prefix + activePrompt + Suffix;
    }

    protected override void CallActivate(InputAction.CallbackContext ctx)
    {
        if (destroyOnActivate)
        {
            helperText.Hide(this);
            InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
        }
        base.CallActivate(ctx);
    }
}