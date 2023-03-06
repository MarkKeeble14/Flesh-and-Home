using UnityEngine;

public class ProgressionTrigger : TextPromptKeyTrigger
{
    [SerializeField] private UnlockType unlock;

    protected override string Suffix => GetUnlockTypeString(unlock);

    private new void Awake()
    {
        onActivate += delegate
        {
            switch (unlock)
            {
                case UnlockType.JETPACK:
                    ProgressionManager._Instance.UnlockJetpack();
                    break;
                case UnlockType.FLAMETHROWER:
                    ProgressionManager._Instance.UnlockFlamethrower();
                    break;
                case UnlockType.LASER_CUTTER:
                    ProgressionManager._Instance.UnlockLaserCutter();
                    break;
                case UnlockType.PULSE_GRENADE_LAUNCHER:
                    ProgressionManager._Instance.UnlockPulseGrenadeLauncher();
                    break;
            }
        };
        base.Awake();
    }

    private string GetUnlockTypeString(UnlockType unlock)
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
                throw new System.Exception("Unhandled Switch Case in Progression Trigger");
        }
    }
}

