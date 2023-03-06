using UnityEngine;

public class StartBossTrigger : TextPromptKeyTrigger
{
    [SerializeField] private BossPhaseManager bossPhaseManager;
    [SerializeField] private OpenDoorTrigger enterDoor;
    [SerializeField] private OpenDoorTrigger exitDoor;

    private new void Awake()
    {
        onActivate += delegate
        {
            bossPhaseManager.EnterCurrentState();
            enterDoor.LockClosed();
            exitDoor.LockClosed();
        };

        base.Awake();
    }
}
