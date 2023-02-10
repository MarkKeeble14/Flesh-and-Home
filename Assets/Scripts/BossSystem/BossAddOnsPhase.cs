using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossAddOnsPhase : BossPhaseBaseState
{
    [Header("Phase")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private CinemachineImpulseSource groundShakeImpulseSource;
    [SerializeField] private float impulseForce;
    [SerializeField] private bool riseBeforeEnding = true;

    [Header("Settings")]
    [SerializeField] private float dropSpeed = .5f;
    [SerializeField] private AnimationCurve dropCurve;
    [SerializeField] private float riseSpeed = .75f;
    [SerializeField] private float reRiseSpeed = .25f;
    [SerializeField] private AnimationCurve reRiseCurve;
    [SerializeField] private float riseDuration = 4f;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private float shakeSpeed = 5f;
    [SerializeField] private float shakeStrength = .02f;

    [Header("Audio")]
    [SerializeField] private AudioSource shakeSource;
    [SerializeField] private AudioSource constantNoiseSource;
    [SerializeField] private AudioClipContainer startShakyRiseClip;
    [SerializeField] private AudioClipContainer finishShakyRiseClip;
    [SerializeField] private AudioClipContainer crashDownStartClip;
    [SerializeField] private AudioClipContainer reachGroundClip;
    [SerializeField] private AudioClipContainer sucessClip;
    [SerializeField] private AudioClipContainer failureClip;
    [SerializeField] private AudioClipContainer reachResultClip;
    [SerializeField] private AudioClipContainer startRiseClip;
    [SerializeField] private AudioClipContainer finishRiseClip;

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering Adds Phase");

        base.EnterState(boss);
    }

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting Adds Phase");

        base.ExitState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        // Stop shell from moving
        boss.ShellEnemyMovement.Move = false;
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        yield return new WaitForSeconds(enterPhaseTime);

        float time = 0;
        float initialYHeight = transform.position.y;

        // Used for shaking
        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);

        // Audio
        startShakyRiseClip.PlayOneShot(boss.source);

        // Rise a little
        for (float t = 0; t < riseDuration; t += Time.deltaTime)
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
                Time.deltaTime * riseSpeed);

            yield return null;
        }

        // Audio
        finishShakyRiseClip.PlayOneShot(boss.source);
        shakeSource.enabled = false;

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Audio
        crashDownStartClip.PlayOneShot(boss.source);

        // Crash down
        float groundTarget = transform.position.y - boss.ShellEnemyMovement.DistanceToGround;
        while (transform.position.y != groundTarget)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, groundTarget, transform.position.z),
                dropCurve.Evaluate(time) * dropSpeed);
            yield return null;
        }

        // Generate Impulse Source
        groundShakeImpulseSource.GenerateImpulse(impulseForce);

        // Audio
        reachGroundClip.PlayOneShot(boss.source);

        // Audio
        constantNoiseSource.enabled = true;

        // Start Adds Phase
        // Set HP Bar
        boss.HPBar.Set(0, duration);

        // Enable HP Bar
        boss.HPBar.Show();

        // Spawn Enemies
        bool success = false;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Set HP Bar
            boss.HPBar.Set(t, duration);

            // If all enemies are dead, break early & set success to true

            // Search for nearby corpses, suckle them if there

            yield return null;
        }

        // Set HP Bar
        boss.HPBar.Set(duration, duration);

        yield return new WaitUntil(() => boss.HPBar.IsFull);

        // Audio
        constantNoiseSource.enabled = false;

        // Audio
        reachResultClip.PlayOneShot(boss.source);

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Audio
        if (success)
        {
            sucessClip.PlayOneShot(boss.source);
        }
        else
        {
            failureClip.PlayOneShot(boss.source);
        }

        if (riseBeforeEnding)
        {
            // Return to original height
            time = 0;
            // Audio
            startRiseClip.PlayOneShot(boss.source);
            while (transform.position.y < initialYHeight)
            {
                time += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, initialYHeight, transform.position.z),
                    reRiseCurve.Evaluate(time) * reRiseSpeed);
                yield return null;
            }

            // Audio
            finishRiseClip.PlayOneShot(boss.source);
        }

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Switch State
        boss.LoadNextPhase();
    }
}