using UnityEngine;
using Cinemachine;

public class InputManager : MonoBehaviour
{
    public static InputManager _Instance { get; private set; }
    public PlayerInputActions PlayerInputActions;
    [SerializeField] private CinemachineInputProvider mouseInput;
    private void Awake()
    {
        _Instance = this;

        // Set up new input
        PlayerInputActions = new PlayerInputActions();

        EnableInput();
        PlayerInputActions.Player.Escape.Enable();

        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        _Instance = null;
        DisableInput();
        PlayerInputActions.Player.Escape.Disable();
    }

    public void EnableInput()
    {
        // Player controls
        // Move and Look
        PlayerInputActions.Player.Move.Enable();
        PlayerInputActions.Player.Look.Enable();
        mouseInput.enabled = true;

        // Actions
        PlayerInputActions.Player.Sprint.Enable();
        PlayerInputActions.Player.Shoot.Enable();
        PlayerInputActions.Player.FireAttachment.Enable();
        PlayerInputActions.Player.Melee.Enable();
        PlayerInputActions.Player.Jump.Enable();

        // Other
        PlayerInputActions.Player.Interact.Enable();
        PlayerInputActions.Player.MousePosition.Enable();
        PlayerInputActions.Player.Tab.Enable();

        // Hotkeys
        PlayerInputActions.Player.Hotkey1.Enable();
        PlayerInputActions.Player.Hotkey2.Enable();
        PlayerInputActions.Player.Hotkey3.Enable();
    }

    public void DisableInput()
    {
        // Player controls
        // Move and Look
        PlayerInputActions.Player.Move.Disable();
        PlayerInputActions.Player.Look.Disable();
        mouseInput.enabled = false;

        // Actions
        PlayerInputActions.Player.Sprint.Disable();
        PlayerInputActions.Player.Shoot.Disable();
        PlayerInputActions.Player.FireAttachment.Disable();
        PlayerInputActions.Player.Melee.Disable();
        PlayerInputActions.Player.Jump.Disable();

        // Other
        PlayerInputActions.Player.Interact.Disable();
        PlayerInputActions.Player.MousePosition.Disable();
        PlayerInputActions.Player.Tab.Disable();

        // Hotkeys
        PlayerInputActions.Player.Hotkey1.Disable();
        PlayerInputActions.Player.Hotkey2.Disable();
        PlayerInputActions.Player.Hotkey3.Disable();
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
