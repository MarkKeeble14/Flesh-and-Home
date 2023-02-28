using UnityEngine;

public class StartBossTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private BossPhaseManager bossPhaseManager;
    [SerializeField] private OpenDoorTrigger enterDoor;
    [SerializeField] private OpenDoorTrigger exitDoor;

    protected override void Activate()
    {
        bossPhaseManager.EnterCurrentState();
        enterDoor.LockClosed();
        exitDoor.LockClosed();
    }
}
