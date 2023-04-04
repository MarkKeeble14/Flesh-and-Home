using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponAttachmentController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClipContainer equipAttachmentClip;

    [Header("UI")]
    [SerializeField] private Color color;
    public Color Color => color;
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
    public bool Firing { get; protected set; }

    public virtual void EnterState(PlayerAttachmentHandler handler)
    {
        // Debug.Log("Enter State: " + this.GetType().ToString());

        // Audio
        equipAttachmentClip.PlayOneShot(handler.source);

        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed += Fire;
    }

    public virtual void ExitState(PlayerAttachmentHandler handler)
    {
        // Debug.Log("Exit State: " + this.GetType().ToString());

        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed -= Fire;
    }

    public abstract void Fire(InputAction.CallbackContext ctx);
}
