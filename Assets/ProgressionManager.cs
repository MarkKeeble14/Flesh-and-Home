using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressionManager : MonoBehaviour
{
    // Make a singleton for easy access
    public static ProgressionManager _Instance;
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance);
        }
        _Instance = this;
    }

    [Header("Settings")]
    [SerializeField] private float unlockTextDuration = 3f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI onUnlockPopupText;
    [SerializeField] private Transform spawnedTextParent;

    [SerializeField] private RadialMenu weaponAttachmentMenu;
    [SerializeField] private GameObject currentWeaponAttachmentDisplay;
    [SerializeField] private GameObject fuelDisplay;

    [Header("Jetpack")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject hasJetpackIcon;

    [Header("Flamethrower")]
    [SerializeField] private GameObject flamethrowerCrosshair;
    [SerializeField] private RadialMenuButton flamethrowerButton;

    [Header("Laser Cutter")]
    [SerializeField] private GameObject laserCutterCrosshair;
    [SerializeField] private RadialMenuButton laserCutterButton;

    [ContextMenu("Unlock/Jetpack")]
    public void UnlockJetpack()
    {
        // Set fuel display to be active
        fuelDisplay.SetActive(true);

        // Turn on icon so player knows they have jetpack
        hasJetpackIcon.SetActive(true);

        // Allow player to use jetpack
        playerController.AquireJetpack();

        // Spawn text
        StartCoroutine(SpawnText("Jetpack"));
    }

    [ContextMenu("Unlock/Flamethrower")]
    public void UnlockFlamethrower()
    {
        // Set fuel display to be active
        fuelDisplay.SetActive(true);

        // Set weapon display to be active
        currentWeaponAttachmentDisplay.SetActive(true);

        // Add and select new attachment
        weaponAttachmentMenu.AddButtion(flamethrowerButton);
        weaponAttachmentMenu.SetButton(flamethrowerButton);

        // Enable crosshair
        flamethrowerCrosshair.gameObject.SetActive(true);

        // Spawn text
        StartCoroutine(SpawnText("Flamethrower"));
    }

    [ContextMenu("Unlock/LaserCutter")]
    public void UnlockLaserCutter()
    {
        // Set weapon display to be active
        currentWeaponAttachmentDisplay.SetActive(true);

        // Add and select new attachment
        weaponAttachmentMenu.AddButtion(laserCutterButton);
        weaponAttachmentMenu.SetButton(laserCutterButton);

        // Enable crosshair
        laserCutterCrosshair.gameObject.SetActive(true);

        // Spawn text
        StartCoroutine(SpawnText("Laser Cutter"));
    }

    public IEnumerator SpawnText(string unlocked)
    {
        TextMeshProUGUI spawned = Instantiate(onUnlockPopupText, spawnedTextParent);
        spawned.text = "Unlocked: " + unlocked + "!";
        yield return new WaitForSeconds(unlockTextDuration);
        spawned.GetComponent<Animator>().SetTrigger("Done");
    }
}
