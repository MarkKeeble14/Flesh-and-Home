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
    }

}
