using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WeaponAttachmentController : MonoBehaviour
{
    public virtual void EnterState(PlayerAttachmentHandler handler)
    {
        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed += Fire;
    }
    public virtual void ExitState(PlayerAttachmentHandler handler)
    {
        InputManager._Instance.PlayerInputActions.Player.FireAttachment.performed -= Fire;
    }
    public abstract void Fire(InputAction.CallbackContext ctx);
}
