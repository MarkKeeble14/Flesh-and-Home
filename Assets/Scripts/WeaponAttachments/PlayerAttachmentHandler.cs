using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttachmentHandler : MonoBehaviour
{
    [SerializeField] private WeaponAttachmentController currentWeaponAttachment;
    [SerializeField] private NoWeaponAttachment noWeaponAttachment;
    [SerializeField] private FlamethrowerSettings flamethrowerAttachment;
    [SerializeField] private LaserCutterSettings laserCutterAttachment;
    [SerializeField] private LaserBayonetSettings laserBayonetAttachment;

    [Header("Audio")]
    public AudioSource source;

    [SerializeField] private Image attachmentEquippedDisplay;

    private void Start()
    {
        // Set character to initially have no weapon attachment
        currentWeaponAttachment = noWeaponAttachment;

        // Add Controls
        InputManager._Instance.PlayerInputActions.Player.Hotkey1.performed += SwitchToFlamethrower;
        InputManager._Instance.PlayerInputActions.Player.Hotkey2.performed += SwitchToLaserCutter;
        InputManager._Instance.PlayerInputActions.Player.Hotkey3.performed += SwitchToLaserBayonet;
    }

    private void SwitchToFlamethrower(InputAction.CallbackContext ctx)
    {
        SwitchAttatchment(flamethrowerAttachment);
    }

    private void SwitchToLaserCutter(InputAction.CallbackContext ctx)
    {
        SwitchAttatchment(laserCutterAttachment);
    }

    private void SwitchToLaserBayonet(InputAction.CallbackContext ctx)
    {
        SwitchAttatchment(laserBayonetAttachment);
    }

    public void SwitchAttatchment(WeaponAttachmentController attachment)
    {
        // Exit
        currentWeaponAttachment.ExitState(this);

        // Switch
        currentWeaponAttachment = attachment;

        // Enter
        currentWeaponAttachment.EnterState(this);

        // Change Color of UI
        attachmentEquippedDisplay.color = currentWeaponAttachment.Color;
    }
}
