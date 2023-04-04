using UnityEngine;

public class StartBossGameEvent : GameEvent
{
    [SerializeField] private BossPhaseManager bossPhaseManager;
    [SerializeField] private OpenDoorGameEvent enterDoor;
    [SerializeField] private OpenDoorGameEvent exitDoor;

    protected override void Activate()
    {
        bossPhaseManager.EnterCurrentState();
        ProgressionManager._Instance.SetBossFightStarted();

        RoomEnemyStateController[] allEnemies = FindObjectsOfType<RoomEnemyStateController>(true);
        foreach (RoomEnemyStateController enemy in allEnemies)
        {
            if (enemy.TryGetComponent(out KillableEntity killable))
            {
                killable.AcceptDamage = true;
                killable.Damage(killable.CurrentHealth, DamageSource.RIFLE);
            }
        }

        enterDoor.LockClosed();
        exitDoor.LockClosed();
    }
}
