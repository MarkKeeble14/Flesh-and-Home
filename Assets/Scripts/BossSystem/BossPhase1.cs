using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase1 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private Transform armorPlateHolder;
    [HideInInspector] public OverheatableBossComponentEntity[] armorPlating;
    [SerializeField] private int platesNeededToDestroyToAdvance;
    private int platesDestroyed;

    private void Awake()
    {
        armorPlating = armorPlateHolder.GetComponentsInChildren<OverheatableBossComponentEntity>();
        // Loop through plates; add an "on end action" to each (will be called when the object is "ended", so usually destroyed) which will simply remove it from the list
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AddAdditionalOnEndAction(() => platesDestroyed++);
        }
    }

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 1");

        // Disallow all plates from taking damage
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AcceptDamage = false;
        }

        base.EnterState(boss);
    }

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting State 1");

        // Loop through plates; No more can take damage
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AcceptDamage = false;
        }

        base.ExitState(boss);
    }

    public override void UpdateState(BossPhaseManager boss)
    {
        // Set HP Bar
        boss.HPBar.Set(platesDestroyed, platesNeededToDestroyToAdvance);

        base.UpdateState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        Debug.Log("Phase 1 Coroutine Start");

        // Wait for a second
        yield return new WaitForSeconds(enterPhaseTime);

        // Allow all plates to take damage
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AcceptDamage = true;
        }

        // Enable HP Bar
        boss.HPBar.Show();

        // Stop shell from moving
        boss.ShellEnemyMovement.Move = false;
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        while (platesDestroyed < platesNeededToDestroyToAdvance)
        {
            // Attack
            yield return StartCoroutine(CallAttacks(boss));
        }

        // Debug.Log("Waiting for Boss to be Full");

        yield return new WaitUntil(() => boss.HPBar.IsFull);

        // Debug.Log("Boss Bar is Full");

        // Switch State
        boss.LoadNextPhase();
    }
}
