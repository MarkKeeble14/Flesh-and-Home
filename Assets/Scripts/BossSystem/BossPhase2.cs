using System.Collections;
using UnityEngine;

public class BossPhase2 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private KillableBossComponentEntity bossShell;

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 2");

        // And also to set complete to true when done
        bossShell.AddAdditionalOnEndAction(() => complete = true);

        base.EnterState(boss);
    }

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting State 2");

        bossShell.AcceptDamage = false;

        base.ExitState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        Debug.Log("Phase 2 Start");

        // Wait for a second
        yield return new WaitForSeconds(enterPhaseTime);

        // Tell Shell to take damage
        bossShell.AcceptDamage = true;

        // Enable HP Bar
        bossShell.SetHPBar(boss.HPBar);
        boss.HPBar.Show();

        // Has movement
        boss.ShellEnemyMovement.SetMove(true);
        boss.ShellEnemyMovement.EnableNavMeshAgent();

        while (!complete)
        {
            yield return StartCoroutine(CallAttacks(boss, GameManager._Instance.PlayerAimAt));
        }

        // Switch State
        boss.LoadNextPhase();
    }
}
