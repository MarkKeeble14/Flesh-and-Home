using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class TextPromptKeyTrigger : MonoBehaviour
{
    private string prefix = "'E' to ";
    [SerializeField] protected string activePrompt;
    protected virtual string Suffix { get => ""; }
    protected TriggerHelperText helperText;
    protected bool showText = true;

    private void Start()
    {
        // Find Helper Text
        helperText = FindObjectOfType<TriggerHelperText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;

        if (showText)
        {
            helperText.Show(this, GetHelperTextString());
        }

        // Add Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;

    }

    protected string GetHelperTextString()
    {
        return prefix + activePrompt + Suffix;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;

        helperText.Hide(this);

        // Remove Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;

    }

    protected virtual void CallActivate(InputAction.CallbackContext ctx)
    {
        helperText.Hide(this);
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
        Activate();
    }

    protected abstract void Activate();
}
