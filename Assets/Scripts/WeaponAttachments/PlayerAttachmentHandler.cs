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
    [SerializeField] private PulseGrenadeLauncherSettings pulseGrenadeLauncherAttachment;

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
        if (attachmentMenu.IsEmpty) yield break;

        attachmentMenu.gameObject.SetActive(true);

        // Cursor.visible = true;
        Time.timeScale = 0.1f;

        InputManager._Instance.PlayerInputActions.Player.Look.Disable();

        yield return new WaitUntil(() => !InputManager._Instance.PlayerInputActions.Player.Tab.IsPressed());

        // Cursor.visible = false;

        Time.timeScale = 1f;

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

    public void SwitchToPulseGrenadeLauncher()
    {
        SwitchAttatchment(pulseGrenadeLauncherAttachment);
    }

    public void SwitchAttatchment(WeaponAttachmentController attachment)
    {
        // Exit
        if (currentWeaponAttachment != null)
            currentWeaponAttachment.ExitState(this);

        // Switch
        currentWeaponAttachment = attachment;

        // Enter
        currentWeaponAttachment.EnterState(this);

        // Change Color of UI
        attachmentEquippedDisplay.color = currentWeaponAttachment.Color;
    }
}
