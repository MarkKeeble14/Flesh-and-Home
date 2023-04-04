using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class BossIntroPhase : BossPhaseBaseState
{
    [Header("Phase")]
    [SerializeField] private AnimationCurve riseCurve;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float targetBaseOffset;
    [SerializeField] private float shellShakeSpeed = 5f;
    [SerializeField] private float shellShakeStrength = .02f;
    [SerializeField] private float cameraShakeStrength = 3f;

    [Header("References")]
    [SerializeField] private AudioClipContainer startRiseClip;
    [SerializeField] private AudioClipContainer finishRiseClip;

    [SerializeField] private float exitTime;

    public override void ExitState(BossPhaseManager boss)
    {
        // Debug.Log("Exiting Boss Intro Phase");

        base.ExitState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        // Debug.Log("Intro Phase Coroutine Start");

        // No movement
        boss.ShellEnemyMovement.SetMove(false);

        // Wait for a second
        yield return new WaitForSeconds(enterPhaseTime);

        yield return StartCoroutine(InitRise(boss, riseCurve, riseSpeed, targetBaseOffset, shellShakeSpeed, shellShakeStrength, cameraShakeStrength, startRiseClip, finishRiseClip));

        if (waitForIdleDialogueBeforeExit)
        {
            // Debug.Log("Waiting for Dialogue");
            yield return new WaitUntil(() => DialogueManager._Instance.Idle);
            // Debug.Log("Done Waiting for Dialogue");
        }

        yield return new WaitForSeconds(exitTime);

        // Switch State
        boss.LoadNextPhase();
    }
}