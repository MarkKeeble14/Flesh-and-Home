using UnityEngine;

public class StartBossTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private BossPhaseManager bossPhaseManager;

    protected override void Activate()
    {
        bossPhaseManager.EnterCurrentState();
    }
}
