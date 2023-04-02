using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    // Make a singleton for easy access
    public static ProgressionManager _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            // Debug.Log("New Progression Manager Set");
            _Instance = this;

            if (reset)
            {
                // Debug.Log("Reset Progression");
                PlayerPrefs.SetInt(flamethrowerUnlockedKey, 0);
                PlayerPrefs.SetInt(jetpackUnlockedKey, 0);
                PlayerPrefs.SetInt(laserCutterUnlockedKey, 0);
                PlayerPrefs.SetInt(pulseGrenadeUnlockedKey, 0);
                PlayerPrefs.SetInt(inBossFightKey, 0);

                reset = false;
            }
            else
            {
                // Debug.Log("Did Not Reset Progression");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static bool reset = true;

    private bool jetpackUnlocked;
    private bool flamethrowerUnlocked;

    private List<UnlockType> currentUnlocks = new List<UnlockType>();

    private bool laserCutterUnlocked;
    private bool pulseGrenadeLauncherUnlocked;

    [Header("Settings")]
    [SerializeField] private bool unlockAllOnStart;
    [SerializeField] private UnlockType[] unlockOnStart;
    [SerializeField] private bool shouldEquipOnStart;
    [SerializeField] private UnlockType equipOnStart;

    [SerializeField] private float unlockTextDuration = 3f;

    [SerializeField] private RadialMenu weaponAttachmentMenu;
    [SerializeField] private GameObject currentWeaponAttachmentDisplay;
    [SerializeField] private GameObject fuelDisplay;

    [Header("Jetpack")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject hasJetpackIcon;
    [SerializeField] private GameObject jetpackSliderBar;

    [Header("Flamethrower")]
    [SerializeField] private GameObject flamethrowerCrosshair;
    [SerializeField] private RadialMenuButton flamethrowerButton;

    [Header("Laser Cutter")]
    [SerializeField] private GameObject laserCutterCrosshair;
    [SerializeField] private RadialMenuButton laserCutterButton;

    [Header("Pulse Grenade Launcher")]
    [SerializeField] private GameObject pulseGrenadeLauncherCrosshair;
    [SerializeField] private RadialMenuButton pulseGrenadeLauncherButton;

    private ChangeSpawnPosition changeSpawnPosition;
    private string jetpackUnlockedKey = "jetpackUnlocked";
    private string flamethrowerUnlockedKey = "flamethrowerUnlocked";
    private string laserCutterUnlockedKey = "laserCutterUnlocked";
    private string pulseGrenadeUnlockedKey = "pulseGrenadeUnlocked";
    private string inBossFightKey = "inBossFight";

    [SerializeField] private FloatStore hp;
    [SerializeField] private FloatStore fuel;

    [SerializeField] private GameObject cameraHintTriggerUpperDoor;
    [SerializeField] private OpenDoorGameEvent upperDoor;
    [SerializeField] private OpenDoorGameEvent bossDoor;

    private void Start()
    {
        playerHealth = playerController.GetComponent<PlayerHealth>();
        changeSpawnPosition = playerController.transform.parent.GetComponent<ChangeSpawnPosition>();

        // Unlock everything on awake for testing
        if (unlockAllOnStart)
        {
            UnlockAll(false);
        }

        foreach (UnlockType unlock in unlockOnStart)
        {
            switch (unlock)
            {
                case UnlockType.FLAMETHROWER:
                    UnlockFlamethrower(false);
                    break;
                case UnlockType.LASER_CUTTER:
                    UnlockLaserCutter(false);
                    break;
                case UnlockType.PULSE_GRENADE_LAUNCHER:
                    UnlockPulseGrenadeLauncher(false);
                    break;
                case UnlockType.JETPACK:
                    UnlockJetpack(false);
                    break;
            }
        }

        if (shouldEquipOnStart)
        {
            switch (equipOnStart)
            {
                case UnlockType.FLAMETHROWER:
                    weaponAttachmentMenu.SetButton(flamethrowerButton);
                    break;
                case UnlockType.LASER_CUTTER:
                    weaponAttachmentMenu.SetButton(laserCutterButton);
                    break;
                case UnlockType.PULSE_GRENADE_LAUNCHER:
                    weaponAttachmentMenu.SetButton(pulseGrenadeLauncherButton);
                    break;
                default:
                    break;
            }
        }
    }

    [ContextMenu("Unlock/Flamethrower")]
    public void UnlockFlamethrower()
    {
        UnlockFlamethrower(false);
    }

    [ContextMenu("Unlock/LaserCutter")]
    public void UnlockLaserCutter()
    {
        UnlockLaserCutter(false);
    }

    [ContextMenu("Unlock/PulseGrenadeLauncher")]
    public void UnlockPulseGrenadeLauncher()
    {
        UnlockPulseGrenadeLauncher(false);
    }

    [ContextMenu("Unlock/Jetpack")]
    public void UnlockJetpack()
    {
        UnlockJetpack(false);
    }


    [ContextMenu("Unlock/All")]
    private void UnlockAll(bool showText)
    {
        UnlockJetpack(showText);
        UnlockFlamethrower(showText);
        UnlockLaserCutter(showText);
        UnlockPulseGrenadeLauncher(showText);
    }

    public void UnlockJetpack(bool showText)
    {
        if (jetpackUnlocked) return;
        jetpackUnlocked = true;

        // Set fuel display to be active
        fuelDisplay.SetActive(true);

        jetpackSliderBar.SetActive(true);

        // Turn on icon so player knows they have jetpack
        // hasJetpackIcon.SetActive(true);

        // Allow player to use jetpack
        playerController.AquireJetpack();

        currentUnlocks.Add(UnlockType.JETPACK);

        GameManager._Instance.PlayerUseFuel = true;

        // Spawn text
        if (showText)
            TextPopupManager._Instance.SpawnText("Jetpack\nPress space when in the air to fly", unlockTextDuration);

        SaveUnlockToPlayerPrefs(jetpackUnlockedKey);
    }

    public void UnlockFlamethrower(bool showText)
    {
        if (flamethrowerUnlocked) return;
        flamethrowerUnlocked = true;

        // Set fuel display to be active
        fuelDisplay.SetActive(true);

        // Set weapon display to be active
        currentWeaponAttachmentDisplay.SetActive(true);

        // Add new attachment
        weaponAttachmentMenu.AddButtion(flamethrowerButton);

        // Enable crosshair
        flamethrowerCrosshair.gameObject.SetActive(true);

        currentUnlocks.Add(UnlockType.FLAMETHROWER);

        GameManager._Instance.PlayerUseFuel = true;

        // Spawn text
        if (showText)
            TextPopupManager._Instance.SpawnText("Unlocked the Flamethrower\nHold Tab to Select, Hold Q to Use", unlockTextDuration);

        SaveUnlockToPlayerPrefs(flamethrowerUnlockedKey);
    }

    public void UnlockLaserCutter(bool showText)
    {
        if (laserCutterUnlocked) return;
        laserCutterUnlocked = true;

        // Set weapon display to be active
        currentWeaponAttachmentDisplay.SetActive(true);

        // Add new attachment
        weaponAttachmentMenu.AddButtion(laserCutterButton);

        // Enable crosshair
        laserCutterCrosshair.gameObject.SetActive(true);

        currentUnlocks.Add(UnlockType.LASER_CUTTER);

        // Spawn text
        if (showText)
            TextPopupManager._Instance.SpawnText("Unlocked the Laser Cutter\nYou can now laser through Metal Platings", unlockTextDuration);

        SaveUnlockToPlayerPrefs(laserCutterUnlockedKey);
    }

    public void UnlockPulseGrenadeLauncher(bool showText)
    {
        if (pulseGrenadeLauncherUnlocked) return;
        pulseGrenadeLauncherUnlocked = true;

        // Set weapon display to be active
        currentWeaponAttachmentDisplay.SetActive(true);

        // Add new attachment
        weaponAttachmentMenu.AddButtion(pulseGrenadeLauncherButton);

        // Enable crosshair
        pulseGrenadeLauncherCrosshair.gameObject.SetActive(true);

        currentUnlocks.Add(UnlockType.PULSE_GRENADE_LAUNCHER);

        // Spawn text
        if (showText)
            TextPopupManager._Instance.SpawnText("Pulse Grenade Launcher\nArea of effect damage", unlockTextDuration);

        SaveUnlockToPlayerPrefs(pulseGrenadeUnlockedKey);
    }

    private void SaveUnlockToPlayerPrefs(string key)
    {
        PlayerPrefs.SetInt(key, 1);
    }

    public bool HasAllUnlocks(List<UnlockType> requiredUnlocks)
    {
        foreach (UnlockType type in requiredUnlocks)
        {
            if (!currentUnlocks.Contains(type)) return false;
        }
        return true;
    }

    public void Unlock(UnlockType unlock, bool showText)
    {
        switch (unlock)
        {
            case UnlockType.JETPACK:
                UnlockJetpack(showText);
                break;
            case UnlockType.FLAMETHROWER:
                UnlockFlamethrower(showText);
                break;
            case UnlockType.LASER_CUTTER:
                UnlockLaserCutter(showText);
                break;
            case UnlockType.PULSE_GRENADE_LAUNCHER:
                UnlockPulseGrenadeLauncher(showText);
                break;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    public static string GetUnlockTypeString(UnlockType unlock)
    {
        switch (unlock)
        {
            case UnlockType.JETPACK:
                return "Jetpack";
            case UnlockType.FLAMETHROWER:
                return "Flamethrower";
            case UnlockType.LASER_CUTTER:
                return "Laser Cutter";
            case UnlockType.PULSE_GRENADE_LAUNCHER:
                return "Pulse Grenade Launcher";
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    public bool AllowRestartFromLastCheckpoint
    {
        get
        {
            if (PlayerPrefs.GetInt(jetpackUnlockedKey) == 1)
            {
                return true;
            }
            if (PlayerPrefs.GetInt(laserCutterUnlockedKey) == 1)
            {
                return true;
            }
            if (PlayerPrefs.GetInt(pulseGrenadeUnlockedKey) == 1)
            {
                return true;
            }
            return false;
        }
    }

    public void SetBossFightStarted()
    {
        PlayerPrefs.SetInt(inBossFightKey, 1);
    }

    public void RestartFromLastCheckpoint()
    {
        playerController.enabled = false;
        TransitionManager._Instance.FadeOut(delegate
        {
            GameManager._Instance.CloseLoseScreen();
            hp.Reset();
            fuel.Reset();

            // Set Change Position
            SpawnPosition spawningAt = SpawnPosition.RUIN_START;
            if (PlayerPrefs.GetInt(jetpackUnlockedKey) == 1)
            {
                spawningAt = SpawnPosition.HUB;
                if (cameraHintTriggerUpperDoor != null)
                    cameraHintTriggerUpperDoor.transform.position = changeSpawnPosition.GetSpawnPosition(SpawnPosition.HUB);
            }
            if (PlayerPrefs.GetInt(laserCutterUnlockedKey) == 1)
            {
                spawningAt = SpawnPosition.HUB;
            }
            if (PlayerPrefs.GetInt(pulseGrenadeUnlockedKey) == 1)
            {
                spawningAt = SpawnPosition.HUB;
                bossDoor.LockOpened();
            }
            if (PlayerPrefs.GetInt(inBossFightKey) == 1)
            {
                spawningAt = SpawnPosition.BOSS_ROOM;
            }
            changeSpawnPosition.ChangePosition(spawningAt);

            TransitionManager._Instance.FadeIn(delegate
            {
                InputManager._Instance.EnableInput();
                playerController.enabled = true;
                playerHealth.AcceptDamage = true;
            });
        });
    }
}
