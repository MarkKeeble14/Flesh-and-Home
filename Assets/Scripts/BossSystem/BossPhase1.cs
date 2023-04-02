using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase1 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private int platesNeededToDestroyToAdvance;

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 1");

        // Disallow all plates from taking damage
        foreach (OverheatableBossComponentEntity plate in boss.ArmorPlating)
        {
            plate.AcceptDamage = false;
        }

        base.EnterState(boss);
    }

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting State 1");

        base.ExitState(boss);
    }

    public override void UpdateState(BossPhaseManager boss)
    {
        // Set HP Bar
        boss.HPBar.Set(boss.NumPlatesDestroyed, platesNeededToDestroyToAdvance);

        // Set complete
        complete = boss.NumPlatesDestroyed >= platesNeededToDestroyToAdvance;

        base.UpdateState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        Debug.Log("Phase 1 Coroutine Start");

        // Wait for a second
        yield return new WaitForSeconds(enterPhaseTime);

        // Allow all plates to take damage
        foreach (OverheatableBossComponentEntity plate in boss.ArmorPlating)
        {
            plate.AcceptDamage = true;
        }

        // Enable HP Bar
        boss.HPBar.Show();

        // Stop shell from moving
        boss.ShellEnemyMovement.SetMove(false);
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        while (!complete)
        {
            // Attack
            yield return StartCoroutine(CallAttacks(boss, GameManager._Instance.PlayerAimAt));
        }

        // Debug.Log("Waiting for Boss to be Full");

        yield return new WaitUntil(() => boss.HPBar.IsFull);

        foreach (OverheatableBossComponentEntity plate in boss.ArmorPlating)
        {
            plate.AcceptDamage = false;
            plate.CoolOff(5000f);
        }

        // Debug.Log("Boss Bar is Full");

        if (waitForIdleDialogueBeforeExit)
        {
            yield return new WaitUntil(() => DialogueManager._Instance.Idle);
        }

        // Switch State
        boss.LoadNextPhase();
    }
}
