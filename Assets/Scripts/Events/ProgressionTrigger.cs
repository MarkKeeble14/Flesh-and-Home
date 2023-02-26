using UnityEngine;

public class ProgressionTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private UnlockType unlock;

    protected override string Suffix => GetUnlockTypeString(unlock);

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
            default:
                throw new System.Exception("Unhandled Switch Case in Progression Trigger");
        }
    }

    protected override void Activate()
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
        }
    }
}

