using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase3 : BossAttackingPhase
{
    [Header("Phase")]
    [SerializeField] private Transform barrelHolder;
    private List<LaserBarrel> bossBarrels = new List<LaserBarrel>();
    [SerializeField] private KillableBossComponentEntity bossFlesh;
    [SerializeField] private float timeBeforeFleshySpawns = 3f;
    [SerializeField] private float timeAfterFleshyKillBeforeDropBarrels;
    [SerializeField] private BossFleshySpawnerOrb turnOff;

    [Header("Plates")]
    [SerializeField] private Transform parentToUnequipped;
    [SerializeField] private Vector2 platesMinMaxForcePerAxis;
    [SerializeField] private Vector2 platesMinMaxTorquePerAxis;

    [Header("Barrels")]
    [SerializeField] private Vector2 barrelsMinMaxForcePerAxis;
    [SerializeField] private Vector2 barrelsMinMaxTorquePerAxis;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer fleshySpawnClip;

    private void Awake()
    {
        // Add Boss Barrels
        bossBarrels.AddRange(barrelHolder.GetComponentsInChildren<LaserBarrel>());
    }

    public override void EnterState(BossPhaseManager boss)
    {
        Debug.Log("Entering State 3");

        turnOff.Enabled = false;

        // And also to set complete to true when done
        bossFlesh.AddAdditionalOnEndAction(() =>
        {
            complete = true;
        });

        foreach (OverheatableBossComponentEntity ent in boss.ArmorPlating)
        {
            // Get Rigidbody component
            Rigidbody rb = ent.GetComponent<Rigidbody>();

            // Uparent
            rb.transform.parent = parentToUnequipped;

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

        foreach (LaserBarrel b in bossBarrels)
        {
            // Uparent
            b.transform.parent = parentToUnequipped;
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
        boss.ShellEnemyMovement.SetMove(false);
        boss.ShellEnemyMovement.DisableNavMeshAgent();

        // Wait some before spawning fleshy
        yield return new WaitForSeconds(timeBeforeFleshySpawns);

        // Turn on Damage
        bossFlesh.AcceptDamage = true;

        // Make Fleshy Boss Exist
        boss.FleshyEnemyMovement.transform.position = boss.ShellEnemyMovement.transform.position;
        boss.FleshyEnemyMovement.gameObject.SetActive(true);
        boss.FleshyEnemyMovement.SetMove(true);

        // Enable HP Bar
        bossFlesh.SetHPBar(boss.HPBar);
        boss.HPBar.Show();

        // Audio
        fleshySpawnClip.PlayOneShot(boss.source);

        // 
        float bossHealth = bossFlesh.MaxHealth;

        // Condition for this phase is for the player to kill the fleshy enemy
        while (!complete)
        {
            yield return StartCoroutine(CallAttacks(boss, GameManager._Instance.PlayerAimAt));
        }

        // Fill remaining HP bar to make it go away
        boss.HPBar.Set(0, bossHealth);


        foreach (LaserBarrel b in bossBarrels)
        {
            b.Disabled = true;
        }

        yield return new WaitForSeconds(timeAfterFleshyKillBeforeDropBarrels);

        // Turn gravity back on for barrels
        foreach (LaserBarrel b in bossBarrels)
        {
            b.Rigidbody.useGravity = true;
            b.Collider.enabled = true;
        }

        // Destroy all remaining enemies
        AttackingEnemy[] enemies = FindObjectsOfType<AttackingEnemy>();
        foreach (AttackingEnemy enemy in enemies)
        {
            if (enemy.gameObject != boss.gameObject)
            {
                Destroy(enemy.gameObject);
            }
        }

        if (waitForIdleDialogueBeforeExit)
        {
            yield return new WaitUntil(() => DialogueManager._Instance.Idle);
        }

        boss.LoadNextPhase();
    }
}
