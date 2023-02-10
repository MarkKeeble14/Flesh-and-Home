using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase3 : BossAttackingPhase
{
    [Header("Phase")]
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
    [SerializeField] private float timeAfterFleshyKillBeforeDropBarrels;

    [Header("Audio")]
    [SerializeField] private AudioSource shakeSource;
    [SerializeField] private AudioClipContainer crashDownStartClip;
    [SerializeField] private AudioClipContainer reachGroundClip;
    [SerializeField] private AudioClipContainer fleshySpawnClip;

    private void Awake()
    {
        // Add Boss Barrels
        bossBarrels.AddRange(barrelHolder.GetComponentsInChildren<BossBarrel>());
    }

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 3");

        // And also to set complete to true when done
        bossFlesh.AddAdditionalOnEndAction(() => complete = true);

        foreach (OverheatableBossComponentEntity ent in boss.bossPhase1.armorPlating)
        {
            // Get Rigidbody component
            Rigidbody rb = ent.GetComponent<Rigidbody>();

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

        // Shell gave up on lift
        boss.ShellEnemyMovement.Move = false;
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        // Wait some before spawning fleshy
        yield return new WaitForSeconds(timeBeforeFleshySpawns);

        // Turn on Damage
        bossFlesh.AcceptDamage = true;

        // Make Fleshy Boss Exist
        boss.FleshyEnemyMovement.transform.position = boss.ShellEnemyMovement.transform.position;
        boss.FleshyEnemyMovement.gameObject.SetActive(true);
        boss.FleshyEnemyMovement.Move = true;
        boss.FleshyEnemyMovement.EnableNavMeshAgent();

        // Enable HP Bar
        bossFlesh.SetHPBar(boss.HPBar);
        boss.HPBar.Show();

        // Audio
        fleshySpawnClip.PlayOneShot(boss.source);

        // Condition for this phase is for the player to kill the fleshy enemy
        while (!complete)
        {
            yield return StartCoroutine(CallAttacks(boss));
        }

        foreach (BossBarrel b in bossBarrels)
        {
            b.Disabled = true;
        }

        // Make boss cease life
        Destroy(bossFlesh.gameObject);

        yield return new WaitForSeconds(timeAfterFleshyKillBeforeDropBarrels);

        // Turn gravity back on for barrels
        foreach (BossBarrel b in bossBarrels)
        {
            b.Rigidbody.useGravity = true;
            b.Collider.enabled = true;
        }

        boss.LoadNextPhase();
    }
}
