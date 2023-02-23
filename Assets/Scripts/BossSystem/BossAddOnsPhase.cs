using System.Collections;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class BossAddOnsPhase : BossPhaseBaseState
{
    [Header("Phase")]
    [SerializeField] private float duration = 10f;
    [SerializeField] protected int numAttacksAtOnce = 1;
    [SerializeField] private Attack onCrashDownAttack;
    [SerializeField] private float timeBetweenAttacks;

    [Header("Spawning")]
    [SerializeField] private PercentageMap<EndableEntity> availableAddOns;
    private List<BossSpawnPoint> spawnPoints = new List<BossSpawnPoint>();
    [SerializeField] private Vector2 minMaxNumAddOns = new Vector2(1, 1);
    [SerializeField] private float timeAddedOnKillEnemy;
    [SerializeField] private NavMeshEnemy fleshTravellingPrefab;

    [Header("Settings")]
    [SerializeField] private float groundShakeImpulseForce = 1;
    [SerializeField] private bool riseBeforeEnding = true;

    [SerializeField] private float pauseDuration = 1f;

    [SerializeField] private float riseSpeed = .75f;
    [SerializeField] private float riseDuration = 4f;
    [SerializeField] private AnimationCurve riseCurve;

    [SerializeField] private float dropSpeed = .5f;
    [SerializeField] private AnimationCurve dropCurve;

    [SerializeField] private float reRiseSpeed = .25f;
    [SerializeField] private AnimationCurve reRiseCurve;

    [SerializeField] private float shakeSpeed = 5f;
    [SerializeField] private float shakeStrength = .02f;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer startCrashClip;
    [SerializeField] private AudioClipContainer finishCrashClip;

    [SerializeField] private AudioClipContainer startRiseClip;
    [SerializeField] private AudioClipContainer finishRiseClip;

    [SerializeField] private AudioSource constantNoiseSource;
    [SerializeField] private AudioClipContainer reachResultClip;
    [SerializeField] private AudioClipContainer sucessClip;
    [SerializeField] private AudioClipContainer failureClip;

    private void Awake()
    {
        // Add all boss spawn points
        spawnPoints.AddRange(FindObjectsOfType<BossEnemySpawnPoint>());
    }

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
        boss.ShellEnemyMovement.SetMove(false);
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        yield return new WaitForSeconds(enterPhaseTime);

        float initialYHeight = transform.position.y;

        yield return StartCoroutine(CommonRoutineRise(boss, riseCurve, riseSpeed, riseDuration, shakeSpeed, shakeStrength, startRiseClip, finishRiseClip));

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        yield return StartCoroutine(CommonRoutineCrash(boss, dropCurve, dropSpeed, groundShakeImpulseForce, startCrashClip, finishCrashClip));

        // Audio
        constantNoiseSource.enabled = true;

        // Start Adds Phase
        // Set HP Bar
        boss.HPBar.Set(0, duration);

        // Enable HP Bar
        boss.HPBar.Show();

        // Spawn Enemies
        int numAdds = RandomHelper.RandomIntInclusive(minMaxNumAddOns);
        // Debug.Log("Number of adds: " + numAdds);
        float time = 0;
        List<EndableEntity> spawnedAdds = new List<EndableEntity>();
        int numAddsToSpawn = 0;
        for (int i = 0; i < numAdds; i++)
        {
            // Get a spawn point
            BossSpawnPoint point = GetSpawnPoint();
            if (point == null)
            {
                // No more spawn points left, should just stop spawning here
                // Debug.Log("No more spawn points: " + i);
                break;
            }
            // say that we've spawned on this location, so it should not be spawnable anymore
            point.SpawnedOn = true;
            // remove the spawn point from the list; need to do so in order to efficiently pick random spawn points
            spawnPoints.Remove(point);
            // track the enemies to be spawned
            numAddsToSpawn++;

            // Spawn Travelling flesh
            NavMeshEnemy traveller = Instantiate(fleshTravellingPrefab, transform.position, Quaternion.identity);
            traveller.EnableNavMeshAgent();

            // Tell that flesh to go to the chosen spawn point
            // Once there, the code inside of the delegate will execute
            StartCoroutine(traveller.GoToOverridenTarget(point.transform.position, 1f, true, false, true, delegate
            {
                // Spawn an Add On
                EndableEntity spawned = Instantiate(availableAddOns.GetOption(), traveller.transform.position, Quaternion.identity);

                // When the add on ends (dies either thru heat or hp), the code inside the delegate will run
                spawned.AddAdditionalOnEndAction(delegate
                {
                    // re-add the spawn point
                    spawnPoints.Add(point);

                    // make the progress bar go a little faster
                    time += timeAddedOnKillEnemy;

                    // track the enemies killed
                    spawnedAdds.Remove(spawned);
                });
                spawnedAdds.Add(spawned);
            }));
        }

        yield return StartCoroutine(onCrashDownAttack.StartAttack(GameManager._Instance.PlayerAimAt, this));

        // Wait until all enemies have been spawned
        yield return new WaitUntil(() => spawnedAdds.Count == numAddsToSpawn);

        // Main Loop
        bool success = false;
        float timer = timeBetweenAttacks;
        for (; time < duration; time += Time.deltaTime)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                for (int i = 0; i < numAttacksAtOnce; i++)
                {
                    StartCoroutine(StartAttack(GameManager._Instance.PlayerAimAt, true));
                }

                timer = timeBetweenAttacks;
            }

            // Set HP Bar
            boss.HPBar.Set(time, duration);

            // If all enemies are dead, break early & set success to true
            if (spawnedAdds.Count <= 0 && numAddsToSpawn > 0)
            {
                time = duration;
                success = true;
                break;
            }

            // Search for nearby corpses, suckle them if there

            yield return null;
        }

        // Set HP Bar
        boss.HPBar.Set(duration, duration);

        // Call on end on all entities to destroy and to repopulate spawn points
        while (spawnedAdds.Count > 0)
        {
            spawnedAdds[0].CallOnEndAction();
        }

        // Set all spawn points to be not spawned on so that they can be reused
        foreach (BossSpawnPoint spawnPoint in spawnPoints) { spawnPoint.SpawnedOn = false; }

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
            yield return StartCoroutine(CommonRoutineRiseTo(boss, reRiseCurve, reRiseSpeed, initialYHeight, shakeSpeed, shakeStrength, groundShakeImpulseForce,
                startRiseClip, finishRiseClip));
        }

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Switch State
        boss.LoadNextPhase();
    }

    private BossSpawnPoint GetSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;
        BossSpawnPoint point = RandomHelper.GetRandomFromList(spawnPoints);
        return point;
    }
}