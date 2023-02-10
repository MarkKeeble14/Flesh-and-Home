using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponAttachmentController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClipContainer equipAttachmentClip;

    [Header("UI")]
    [SerializeField] private Color color;
    public Color Color => color;

    public virtual void EnterState(PlayerAttachmentHandler handler)
    {
        // Audio
        equipAttachmentClip.PlayOneShot(handler.source);

        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed += Fire;
    }

    public virtual void ExitState(PlayerAttachmentHandler handler)
    {
        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed -= Fire;
    }

    public abstract void Fire(InputAction.CallbackContext ctx);
}
