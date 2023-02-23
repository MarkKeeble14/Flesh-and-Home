using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class TextPromptKeyTrigger : MonoBehaviour
{
    [SerializeField] protected string activePrompt;
    protected TriggerHelperText helperText;

    private void Start()
    {
        // Find Helper Text
        helperText = FindObjectOfType<TriggerHelperText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;

        helperText.gameObject.SetActive(true);
        helperText.Show(activePrompt);

        // Add Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;

        helperText.Hide();

        // Remove Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
    }

    protected virtual void CallActivate(InputAction.CallbackContext ctx)
    {
        helperText.Hide();
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
        Activate();
    }

    protected abstract void Activate();
}
