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
    [SerializeField] private AudioClipContainer onSwitchClip;

    [SerializeField] private Image attachmentEquippedDisplay;
    [SerializeField] private RadialMenu attachmentMenu;

    private void Start()
    {
        // Set character to initially have no weapon attachment
        currentWeaponAttachment = noWeaponAttachment;

        // Add Controls
        InputManager._Instance.PlayerInputActions.Player.Tab.performed += OpenAttachmentMenu;
    }

    private void OpenAttachmentMenu(InputAction.CallbackContext ctx)
    {
        StartCoroutine(OpenAttachmentMenu());
    }

    private IEnumerator OpenAttachmentMenu()
    {
        attachmentMenu.gameObject.SetActive(true);

        Cursor.visible = true;

        InputManager._Instance.PlayerInputActions.Player.Look.Disable();

        yield return new WaitUntil(() => !InputManager._Instance.PlayerInputActions.Player.Tab.IsPressed());

        Cursor.visible = false;

        InputManager._Instance.PlayerInputActions.Player.Look.Enable();

        attachmentMenu.SelectButton();

        onSwitchClip.PlayOneShot(source);

        attachmentMenu.gameObject.SetActive(false);
    }

    public void SwitchToFlamethrower()
    {
        SwitchAttatchment(flamethrowerAttachment);
    }

    public void SwitchToLaserCutter()
    {
        SwitchAttatchment(laserCutterAttachment);
    }

    public void SwitchToLaserBayonet()
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
