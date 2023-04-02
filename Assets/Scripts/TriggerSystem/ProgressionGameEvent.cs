using UnityEngine;

public class ProgressionGameEvent : GameEvent
{
    [SerializeField] private UnlockType unlock;

    public override string EventString { get => ProgressionManager.GetUnlockTypeString(unlock); }

    protected override void Activate()
    {
        // Debug.Log("Activating");
        ProgressionManager._Instance.Unlock(unlock, true);
    }
}
