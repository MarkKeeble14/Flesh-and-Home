using UnityEngine;

public class StartBossTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private BossPhaseManager bossPhaseManager;
    [SerializeField] private OpenDoorTrigger bossDoor;

    protected override void Activate()
    {
        bossPhaseManager.EnterCurrentState();
        bossDoor.LockClosed();
    }
}
