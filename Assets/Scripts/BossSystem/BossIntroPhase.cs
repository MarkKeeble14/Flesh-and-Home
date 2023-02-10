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
    [SerializeField] private NavMeshAgent shellNavMeshAgent;
    [SerializeField] private float shellShakeSpeed = 5f;
    [SerializeField] private float shellShakeStrength = .02f;

    [SerializeField] private float cameraShakeStrength = 3f;

    [Header("References")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Audio")]
    [SerializeField] private AudioSource shakingClip;
    [SerializeField] private AudioClipContainer startRiseClip;
    [SerializeField] private AudioClipContainer reachBaseOffsetClip;

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting Boss Intro Phase");

        base.ExitState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        Debug.Log("Intro Phase Coroutine Start");

        // No movement
        boss.ShellEnemyMovement.Move = false;

        // Wait for a second
        yield return new WaitForSeconds(enterPhaseTime);

        // Return to origional height
        float time = 0;

        // Audio
        startRiseClip.PlayOneShot(boss.source);

        // Used for shaking
        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);
        while (shellNavMeshAgent.baseOffset < targetBaseOffset)
        {
            time += Time.deltaTime;

            if (Time.timeScale != 0)
            {
                // Audio
                shakingClip.enabled = true;

                // Shake Object
                TransformHelper.ShakeTransform(boss.ShellEnemyMovement.transform, randStartTimeX, randStartTimeZ, shellShakeSpeed, shellShakeStrength);

                // Shake Screen
                impulseSource.GenerateImpulse(cameraShakeStrength);
            }
            else
            {
                // Audio
                shakingClip.enabled = false;
            }

            shellNavMeshAgent.baseOffset = Mathf.MoveTowards(shellNavMeshAgent.baseOffset, targetBaseOffset, riseCurve.Evaluate(time) * riseSpeed);
            yield return null;
        }

        // Audio
        reachBaseOffsetClip.PlayOneShot(boss.source);
        shakingClip.enabled = false;

        // Switch State
        boss.LoadNextPhase();
    }
}
