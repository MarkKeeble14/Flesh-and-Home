using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase1 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private Transform armorPlateHolder;
    [HideInInspector] public List<OverheatableBossComponentEntity> armorPlating = new List<OverheatableBossComponentEntity>();
    [SerializeField] private int platesNeededToDestroyToAdvance;
    private int platesDestroyed;

    private void Awake()
    {
        armorPlating.AddRange(armorPlateHolder.GetComponentsInChildren<OverheatableBossComponentEntity>());
        // Loop through plates; add an "on end action" to each (will be called when the object is "ended", so usually destroyed) which will simply remove it from the list
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AddAdditionalOnEndAction(() =>
            {
                armorPlating.Remove(plate);
                platesDestroyed++;
                if (platesDestroyed >= platesNeededToDestroyToAdvance) complete = true;
            });
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
        boss.ShellEnemyMovement.SetMove(false);
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        while (!complete)
        {
            // Attack
            yield return StartCoroutine(CallAttacks(boss, GameManager._Instance.PlayerAimAt));
        }

        // Debug.Log("Waiting for Boss to be Full");

        yield return new WaitUntil(() => boss.HPBar.IsFull);

        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AcceptDamage = false;
            plate.CoolOff(200f);
        }

        // Debug.Log("Boss Bar is Full");

        // Switch State
        boss.LoadNextPhase();
    }
}
