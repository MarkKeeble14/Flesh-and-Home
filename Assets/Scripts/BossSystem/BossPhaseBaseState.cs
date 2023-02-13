using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossPhaseBaseState : MonoBehaviour
{
    [Header("Phase")]
    [SerializeField] protected float enterPhaseTime = 2.5f;
    protected bool complete;

    [Header("Bar Settings")]
    [SerializeField] protected ImageSliderBarSettings phaseBarSettings;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onEnterPhaseClip;
    [SerializeField] private AudioClipContainer onExitPhaseClip;

    [Header("References")]
    [SerializeField] private NavMeshAgent shellNavMeshAgent;
    [SerializeField] private CinemachineImpulseSource shakeImpulseSource;
    [SerializeField] private CinemachineImpulseSource groundShakeImpulseSource;

    [Header("Audio")]
    [SerializeField] private AudioSource shakeSource;

    public virtual void EnterState(BossPhaseManager boss)
    {
        // Audio
        onEnterPhaseClip.PlayOneShot(boss.source);

        // Start Phase Behaviour
        StartCoroutine(StateBehaviour(boss));

        // Set Bar Settings
        boss.HPBar.SetFromSettings(phaseBarSettings);
    }

    protected abstract IEnumerator StateBehaviour(BossPhaseManager boss);
    public virtual void UpdateState(BossPhaseManager boss)
    {
        // 
    }
    public virtual void ExitState(BossPhaseManager boss)
    {
        // Disable HP Bar
        boss.HPBar.gameObject.SetActive(false);

        // Audio
        onExitPhaseClip.PlayOneShot(boss.source);
    }

    protected IEnumerator CommonRoutineRise(BossPhaseManager boss, AnimationCurve curve, float speed, float duration, float shakeSpeed, float shakeStrength,
        AudioClipContainer startRiseClip, AudioClipContainer finishRiseClip)
    {
        // Used for shaking
        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);

        // Audio
        startRiseClip.PlayOneShot(boss.source);

        // Rise a little
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Shake
            if (Time.timeScale != 0)
            {
                // Shake Object
                TransformHelper.ShakeTransform(transform, randStartTimeX, randStartTimeZ, shakeSpeed, shakeStrength);

                // Audio
                shakeSource.enabled = true;
            }
            else
            {
                shakeSource.enabled = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y + transform.up.y, transform.position.z),
                curve.Evaluate(t) * speed);

            yield return null;
        }

        // Audio
        finishRiseClip.PlayOneShot(boss.source);
        shakeSource.enabled = false;
    }

    protected IEnumerator CommonRoutineRiseTo(BossPhaseManager boss, AnimationCurve curve, float speed, float target, float shakeSpeed, float shakeStrength, float cameraShakeStrength,
        AudioClipContainer startRiseClip, AudioClipContainer finishRiseClip)
    {
        // Return to origional height
        float time = 0;

        // Audio
        startRiseClip.PlayOneShot(boss.source);

        // Used for shaking
        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);
        while (transform.position.y < target)
        {
            time += Time.deltaTime;

            if (Time.timeScale != 0)
            {
                // Audio
                shakeSource.enabled = true;

                // Shake Object
                TransformHelper.ShakeTransform(boss.ShellEnemyMovement.transform, randStartTimeX, randStartTimeZ, shakeSpeed, shakeStrength);

                // Shake Screen
                shakeImpulseSource.GenerateImpulse(cameraShakeStrength);
            }
            else
            {
                // Audio
                shakeSource.enabled = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, target, transform.position.z), curve.Evaluate(time) * speed);
            yield return null;
        }

        // Audio
        finishRiseClip.PlayOneShot(boss.source);
        shakeSource.enabled = false;
    }

    protected IEnumerator InitRise(BossPhaseManager boss, AnimationCurve curve, float speed, float target, float shakeSpeed, float shakeStrength, float cameraShakeStrength,
        AudioClipContainer startRiseClip, AudioClipContainer finishRiseClip)
    {
        // Return to origional height
        float time = 0;

        // Audio
        startRiseClip.PlayOneShot(boss.source);

        // Used for shaking
        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);
        while (shellNavMeshAgent.baseOffset < target)
        {
            time += Time.deltaTime;

            if (Time.timeScale != 0)
            {
                // Audio
                shakeSource.enabled = true;

                // Shake Object
                TransformHelper.ShakeTransform(boss.ShellEnemyMovement.transform, randStartTimeX, randStartTimeZ, shakeSpeed, shakeStrength);

                // Shake Screen
                shakeImpulseSource.GenerateImpulse(cameraShakeStrength);
            }
            else
            {
                // Audio
                shakeSource.enabled = false;
            }

            shellNavMeshAgent.baseOffset = Mathf.MoveTowards(shellNavMeshAgent.baseOffset, target, curve.Evaluate(time) * speed);
            yield return null;
        }

        // Audio
        finishRiseClip.PlayOneShot(boss.source);
        shakeSource.enabled = false;
    }

    protected IEnumerator CommonRoutineCrash(BossPhaseManager boss, AnimationCurve curve, float speed, float impulseForce,
        AudioClipContainer crashDownStartClip, AudioClipContainer reachGroundClip)
    {
        float time = 0;
        // Audio
        crashDownStartClip.PlayOneShot(boss.source);

        // Crash down
        float groundTarget = transform.position.y - boss.ShellEnemyMovement.DistanceToGround;
        while (transform.position.y != groundTarget)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, groundTarget, transform.position.z),
                curve.Evaluate(time) * speed);
            yield return null;
        }

        // Generate Impulse Source
        groundShakeImpulseSource.GenerateImpulse(impulseForce);

        // Audio
        reachGroundClip.PlayOneShot(boss.source);
    }
}