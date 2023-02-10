using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase3 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private Transform armorPlateHolder;
    private List<Rigidbody> armorPlates = new List<Rigidbody>();

    [SerializeField] private Transform barrelHolder;
    private List<BossBarrel> bossBarrels = new List<BossBarrel>();

    private float timeBeforeFleshySpawns = 3f;
    [SerializeField] private float shakeSpeed = 5f;
    [SerializeField] private float shakeStrength = .02f;
    [SerializeField] private float dropSpeed = .5f;
    [SerializeField] private AnimationCurve dropCurve;

    [Header("Condition")]
    [SerializeField] private KillableBossComponentEntity bossFlesh;
    private bool complete;

    [Header("Plates")]
    [SerializeField] private Transform rotatersHolder;
    [SerializeField] private Vector2 platesMinMaxForcePerAxis;
    [SerializeField] private Vector2 platesMinMaxTorquePerAxis;

    [Header("Barrels")]
    [SerializeField] private Vector2 barrelsMinMaxForcePerAxis;
    [SerializeField] private Vector2 barrelsMinMaxTorquePerAxis;

    [Header("Audio")]
    [SerializeField] private AudioSource shakeSource;
    [SerializeField] private AudioClipContainer crashDownStartClip;
    [SerializeField] private AudioClipContainer reachGroundClip;
    [SerializeField] private AudioClipContainer fleshySpawnClip;


    private void Awake()
    {
        // Add Plates
        armorPlates.AddRange(armorPlateHolder.GetComponentsInChildren<Rigidbody>());

        // Add Boss Barrels
        bossBarrels.AddRange(barrelHolder.GetComponentsInChildren<BossBarrel>());
    }

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 3");

        // Tell Flesh to take damage
        bossFlesh.AcceptDamage = true;
        bossFlesh.SetHPBar(boss.HPBar);

        // And also to set complete to true when done
        bossFlesh.AddAdditionalOnEndAction(() => complete = true);

        foreach (Rigidbody rb in armorPlates)
        {
            // Uparent
            rb.transform.parent = rotatersHolder;

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(new Vector3(
                    Random.Range(platesMinMaxForcePerAxis.x, platesMinMaxForcePerAxis.y),
                    Random.Range(platesMinMaxForcePerAxis.x, platesMinMaxForcePerAxis.y),
                    Random.Range(platesMinMaxForcePerAxis.x, platesMinMaxForcePerAxis.y)
                ), ForceMode.Impulse);
            rb.AddTorque(new Vector3(
                Random.Range(platesMinMaxTorquePerAxis.x, platesMinMaxTorquePerAxis.y),
                Random.Range(platesMinMaxTorquePerAxis.x, platesMinMaxTorquePerAxis.y),
                Random.Range(platesMinMaxTorquePerAxis.x, platesMinMaxTorquePerAxis.y)
            ), ForceMode.Impulse);
        }

        foreach (BossBarrel b in bossBarrels)
        {
            // Uparent
            b.transform.parent = rotatersHolder;
            b.IsAttached = false;

            Rigidbody rb = b.Rigidbody;
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.AddForce(new Vector3(
                    Random.Range(barrelsMinMaxForcePerAxis.x, barrelsMinMaxForcePerAxis.y),
                    Random.Range(barrelsMinMaxForcePerAxis.x, barrelsMinMaxForcePerAxis.y),
                    Random.Range(barrelsMinMaxForcePerAxis.x, barrelsMinMaxForcePerAxis.y)
                ), ForceMode.Impulse);
            rb.AddTorque(new Vector3(
                Random.Range(barrelsMinMaxTorquePerAxis.x, barrelsMinMaxTorquePerAxis.y),
                Random.Range(barrelsMinMaxTorquePerAxis.x, barrelsMinMaxTorquePerAxis.y),
                Random.Range(barrelsMinMaxTorquePerAxis.x, barrelsMinMaxTorquePerAxis.y)
            ), ForceMode.Impulse);
        }

        base.EnterState(boss);
    }

    public override void ExitState(BossPhaseManager boss)
    {
        Debug.Log("Exiting State 3");

        base.ExitState(boss);
    }

    protected override IEnumerator StateBehaviour(BossPhaseManager boss)
    {
        Debug.Log("Phase 3 Start");

        // Stop shell from moving
        boss.ShellEnemyMovement.Move = false;
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        // Used for shaking
        float randX = Random.Range(0, 100);
        float randZ = Random.Range(0, 100);
        for (float t = 0; t < enterPhaseTime; t += Time.deltaTime)
        {
            if (Time.timeScale != 0)
            {
                TransformHelper.ShakeTransform(transform, randX, randZ, shakeSpeed, shakeStrength);
                shakeSource.enabled = true;
            }
            else
            {
                shakeSource.enabled = false;
            }
        }

        // Crash down
        // Audio
        crashDownStartClip.PlayOneShot(boss.source);

        float time = 0;
        float groundTarget = transform.position.y - boss.ShellEnemyMovement.DistanceToGround;
        while (transform.position.y != groundTarget)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, groundTarget, transform.position.z),
                dropCurve.Evaluate(time) * dropSpeed);

            yield return null;
        }

        // Audio
        reachGroundClip.PlayOneShot(boss.source);

        // Turn on Damage
        bossFlesh.AcceptDamage = true;

        // Shell gave up on lift
        boss.ShellEnemyMovement.Move = false;

        // Wait some before spawning fleshy
        yield return new WaitForSeconds(timeBeforeFleshySpawns);

        // Make Fleshy Boss Exist
        boss.FleshyEnemyMovement.transform.position = boss.ShellEnemyMovement.transform.position;
        boss.FleshyEnemyMovement.gameObject.SetActive(true);
        boss.FleshyEnemyMovement.Move = true;
        boss.FleshyEnemyMovement.EnableNavMeshAgent();

        // Enable HP Bar
        boss.HPBar.Show();

        // Audio
        fleshySpawnClip.PlayOneShot(boss.source);

        // Condition for this phase is for the player to kill the fleshy enemy
        while (!complete)
        {
            yield return StartCoroutine(CallAttacks(boss));
        }

        yield return new WaitUntil(() => boss.HPBar.IsFull);

        boss.LoadNextPhase();
    }
}
