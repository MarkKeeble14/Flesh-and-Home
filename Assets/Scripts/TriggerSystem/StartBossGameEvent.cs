using UnityEngine;

public class StartBossGameEvent : GameEvent
{
    [SerializeField] private BossPhaseManager bossPhaseManager;
    [SerializeField] private OpenDoorGameEvent enterDoor;
    [SerializeField] private OpenDoorGameEvent exitDoor;

    protected override void Activate()
    {
        bossPhaseManager.EnterCurrentState();
        enterDoor.LockClosed();
        exitDoor.LockClosed();
    }
}
