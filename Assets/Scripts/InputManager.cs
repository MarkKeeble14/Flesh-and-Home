using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _Instance { get; private set; }
    public PlayerInputActions PlayerInputActions;
    private void Awake()
    {
        if (_Instance != null)
        {
            _Instance.DisableInput();
            Destroy(_Instance.gameObject);
        }
        _Instance = this;

        // Set up new input
        PlayerInputActions = new PlayerInputActions();
        EnableInput();
        Cursor.visible = false;
    }

    public void EnableInput()
    {
        // Player controls
        // Move and Look
        PlayerInputActions.Player.Move.Enable();
        PlayerInputActions.Player.Look.Enable();

        // Actions
        PlayerInputActions.Player.Sprint.Enable();
        PlayerInputActions.Player.FireAttachment.Enable();
        PlayerInputActions.Player.Melee.Enable();
        PlayerInputActions.Player.Jump.Enable();

        // Other
        PlayerInputActions.Player.Interact.Enable();
    }

    public void DisableInput()
    {
        // Player controls
        // Move and Look
        PlayerInputActions.Player.Move.Disable();
        PlayerInputActions.Player.Look.Disable();

        // Actions
        PlayerInputActions.Player.Sprint.Disable();
        PlayerInputActions.Player.FireAttachment.Disable();
        PlayerInputActions.Player.Melee.Disable();
        PlayerInputActions.Player.Jump.Disable();

        // Other
        PlayerInputActions.Player.Interact.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return PlayerInputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return PlayerInputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return PlayerInputActions.Player.Jump.triggered;
    }
}
