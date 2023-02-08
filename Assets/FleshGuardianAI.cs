using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleshGuardianAI : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float[] phaseChangeBreakpoints = new float[3];
    [SerializeField] private float nextHPBreakpoint;
    [SerializeField] private int currentPhase;

    [Header("Lasers")]
    [SerializeField] private float laserRange = 100f;
    [SerializeField] private float laserGrowSpeed;
    [SerializeField] private float laserShrinkSpeedModifier = 4f;
    [SerializeField] private float laserStayDuration = 2f;
    [SerializeField] private float maxLaserWidth = 1f;
    private BossBarrel[][] rotatingBarrelSets;

    [Header("Phase 1")]
    [SerializeField] private float enterPhase1Time = 2.5f;
    [SerializeField] private float phase1TimeBetweenAttacks = 3f;
    [SerializeField] private int maxSingleLaserCalls = 8;

    [Header("Phase 2")]
    [SerializeField] private float phase2TimeBetweenAttacks = 1.5f;
    private BossBarrel[][] stationaryBarrelSets;

    [Header("Phase 3")]
    [SerializeField] private float phase3TimeBetweenAttacks = .5f;
    [SerializeField] private Vector2 minMaxTorquePerAxisOnEnterPhase3 = new Vector2(1, 5);
    [SerializeField] private Vector2 minMaxForcePerAxisOnEnterPhase3 = new Vector2(10, 50);

    [Header("Adds Phase")]
    [SerializeField] private float addsPhaseDuration = 10f;
    [SerializeField] private float dropSpeed;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float reRiseSpeed;
    [SerializeField] private float riseDuration;
    [SerializeField] private float pauseDuration;
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float shakeStrength;

    [Header("Settings")]
    [SerializeField] private float heightOffset = 1f;

    [Header("References")]
    [Header("Shell")]
    [SerializeField] private NavMeshEnemy shellNavMeshEnemy;
    [SerializeField] private NavMeshAgent shellNavMeshAgent;
    [SerializeField] private Rigidbody shellNavMeshRigidbody;
    [Header("Fleshy")]
    [SerializeField] private NavMeshEnemy fleshyBoss;
    [Header("Other Objects")]
    [SerializeField] private ImageSliderBar hpBar;
    [SerializeField] private Transform player;
    [SerializeField] private Transform rotatingBarrelSetsHolder;
    [SerializeField] private Transform stationaryBarrelSetsHolder;

    private void Awake()
    {
        // Set
        currentHealth = maxHealth;
        nextHPBreakpoint = currentHealth;

        // Fetch and add all rotating sets
        rotatingBarrelSets = new BossBarrel[rotatingBarrelSetsHolder.childCount][];
        for (int i = 0; i < rotatingBarrelSetsHolder.childCount; i++)
        {
            Transform child = rotatingBarrelSetsHolder.GetChild(i);
            rotatingBarrelSets[i] = child.GetComponentsInChildren<BossBarrel>();
        }

        // Fetch and add all stationary sets
        stationaryBarrelSets = new BossBarrel[stationaryBarrelSetsHolder.childCount][];
        for (int i = 0; i < stationaryBarrelSetsHolder.childCount; i++)
        {
            Transform child = stationaryBarrelSetsHolder.GetChild(i);
            stationaryBarrelSets[i] = child.GetComponentsInChildren<BossBarrel>();
        }

        StartPhase(false);
    }

    private void Update()
    {
        // Update HP Bar
        hpBar.Set(currentHealth, maxHealth);
    }

    private void StartPhase(bool incrementIndex)
    {
        // Increment the current phase index
        if (incrementIndex)
            currentPhase++;

        // Adjust next HP Breakpoint
        nextHPBreakpoint -= phaseChangeBreakpoints[currentPhase];

        // Start Next Phase, or alternatively die if completed
        switch (currentPhase)
        {
            case 0:
                StartCoroutine(Phase1());
                break;
            case 1:
                StartCoroutine(Phase2());
                break;
            case 2:
                StartCoroutine(Phase3());
                break;
        }
    }


    private IEnumerator Phase1()
    {
        Debug.Log("Phase 1 Start");

        // No movement
        shellNavMeshEnemy.enabled = false;
        shellNavMeshAgent.baseOffset = heightOffset;

        yield return new WaitForSeconds(enterPhase1Time);

        while (currentHealth > nextHPBreakpoint)
        {
            yield return StartCoroutine(RandomLaserAttack());

            yield return new WaitForSeconds(phase1TimeBetweenAttacks);
        }

        // Next Phase
        StartCoroutine(AddsPhase());
    }

    private IEnumerator Phase2()
    {
        Debug.Log("Phase 2 Start");

        // Has movement
        shellNavMeshAgent.enabled = true;
        shellNavMeshEnemy.enabled = true;

        while (currentHealth > nextHPBreakpoint)
        {
            StartCoroutine(CallShootPlayerLaserAttack());

            yield return StartCoroutine(RandomLaserAttack());

            yield return new WaitForSeconds(phase2TimeBetweenAttacks);
        }

        // Next Phase
        StartCoroutine(AddsPhase());
    }

    private IEnumerator Phase3()
    {
        Debug.Log("Phase 3 Start");

        // Shell gave up on lift
        shellNavMeshEnemy.enabled = false;
        shellNavMeshAgent.enabled = false;
        shellNavMeshRigidbody.useGravity = true;
        shellNavMeshRigidbody.isKinematic = false;
        shellNavMeshRigidbody.AddTorque(
            new Vector3(
                Random.Range(minMaxTorquePerAxisOnEnterPhase3.x, minMaxTorquePerAxisOnEnterPhase3.y),
                Random.Range(minMaxTorquePerAxisOnEnterPhase3.x, minMaxTorquePerAxisOnEnterPhase3.y),
                Random.Range(minMaxTorquePerAxisOnEnterPhase3.x, minMaxTorquePerAxisOnEnterPhase3.y)
                ), ForceMode.Impulse
            );
        shellNavMeshRigidbody.AddForce(
            new Vector3(
                Random.Range(minMaxForcePerAxisOnEnterPhase3.x, minMaxForcePerAxisOnEnterPhase3.y),
                Random.Range(minMaxForcePerAxisOnEnterPhase3.y / 2, minMaxForcePerAxisOnEnterPhase3.y),
                Random.Range(minMaxForcePerAxisOnEnterPhase3.x, minMaxForcePerAxisOnEnterPhase3.y)
                ), ForceMode.Impulse
            );

        // Wait some before spawning fleshy
        yield return new WaitForSeconds(3f);

        // Make Fleshy Boss Exist
        fleshyBoss.transform.position = shellNavMeshAgent.transform.position;
        fleshyBoss.gameObject.SetActive(true);

        while (currentHealth > nextHPBreakpoint)
        {
            yield return StartCoroutine(RandomLaserAttack());

            yield return new WaitForSeconds(phase3TimeBetweenAttacks);
        }

        // Make Fleshy Boss Cease to Exist
        Die();
    }

    private IEnumerator AddsPhase()
    {
        shellNavMeshEnemy.enabled = false;
        shellNavMeshAgent.enabled = false;

        float initialYHeight = transform.position.y;

        float randStartTimeX = Random.Range(0, 100f);
        float randStartTimeZ = Random.Range(0, 100f);

        // Rise a little
        for (float t = 0; t < riseDuration; t += Time.deltaTime)
        {
            // Shake
            if (Time.timeScale != 0)
            {
                t += Time.deltaTime;

                // Shake
                float xVal = Mathf.Sin(randStartTimeX + Time.time * shakeSpeed) * shakeStrength;
                float zVal = Mathf.Sin(randStartTimeZ + Time.time * shakeSpeed) * shakeStrength;
                transform.position = new Vector3(
                    transform.position.x + xVal,
                    transform.position.y,
                    transform.position.z + zVal);
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y + transform.up.y, transform.position.z), Time.deltaTime * riseSpeed);

            yield return null;
        }

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Crash down
        float groundTarget = transform.position.y - shellNavMeshEnemy.DistanceToGround;
        while (transform.position.y != groundTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, groundTarget, transform.position.z), Time.deltaTime * dropSpeed);
            yield return null;
        }

        // Start Adds Phase
        // Spawn Enemies

        for (float t = 0; t < addsPhaseDuration; t += Time.deltaTime)
        {
            // If all enemies are dead, break early

            // Search for nearby corpses, suckle them if there

            yield return null;
        }

        // Return to origional height
        while (transform.position.y != initialYHeight)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, initialYHeight, transform.position.z), Time.deltaTime * reRiseSpeed);
            yield return null;
        }

        // Pause
        yield return new WaitForSeconds(pauseDuration);

        // Next Phase
        StartPhase(true);
    }


    private void Die()
    {
        Debug.Log("Die");
        Destroy(hpBar.gameObject);
        Destroy(fleshyBoss.transform.parent.gameObject);
    }

    private IEnumerator RandomLaserAttack()
    {
        int r = Mathf.RoundToInt(Random.Range(0, 10));
        if (r <= 2)
        {
            yield return StartCoroutine(CallSetLaserAttack());
        }
        else if (r <= 4)
        {
            yield return StartCoroutine(CallSingleLaserAttack());
        }
        else
        {
            yield return StartCoroutine(CallMultipleSingleLaserAttack());
        }
    }

    private IEnumerator CallShootPlayerLaserAttack()
    {
        BossBarrel[] set = stationaryBarrelSets[Mathf.RoundToInt(Random.Range(0, stationaryBarrelSets.Length))];

        BossBarrel closest = set[0];
        for (int i = 1; i < stationaryBarrelSets.Length; i++)
        {
            if (Vector3.Distance(player.position, set[i].transform.position) <
                Vector3.Distance(player.position, closest.transform.position))
            {
                closest = set[i];
            }
        }
        yield return StartCoroutine(LaserFrom(closest, (shellNavMeshEnemy.transform.position - closest.transform.position).normalized));
    }

    private IEnumerator CallMultipleSingleLaserAttack()
    {
        int toCall = Random.Range(1, maxSingleLaserCalls);
        List<BossBarrel> callOn = new List<BossBarrel>();

        while (callOn.Count < toCall)
        {
            BossBarrel[] selectedSet = rotatingBarrelSets[Random.Range(0, rotatingBarrelSets.Length)];
            BossBarrel selected = selectedSet[Random.Range(0, selectedSet.Length)];
            if (!callOn.Contains(selected))
            {
                callOn.Add(selected);
            }
        }

        for (int i = 0; i < callOn.Count; i++)
        {
            BossBarrel selected = callOn[i];

            if (i == callOn.Count - 1)
                yield return StartCoroutine(LaserFrom(selected));
            else
                StartCoroutine(LaserFrom(selected));
        }
    }

    private IEnumerator CallSingleLaserAttack()
    {
        BossBarrel[] selectedSet = rotatingBarrelSets[Random.Range(0, rotatingBarrelSets.Length)];
        BossBarrel selected = selectedSet[Random.Range(0, selectedSet.Length)];

        yield return StartCoroutine(LaserFrom(selected));
    }

    private IEnumerator CallSetLaserAttack()
    {
        BossBarrel[] selectedSet = rotatingBarrelSets[Random.Range(0, rotatingBarrelSets.Length)];

        for (int i = 0; i < selectedSet.Length; i++)
        {
            BossBarrel selected = selectedSet[i];

            if (i == selectedSet.Length - 1)
                yield return StartCoroutine(LaserFrom(selected));
            else
                StartCoroutine(LaserFrom(selected));
        }
    }

    private IEnumerator LaserFrom(BossBarrel selected)
    {
        Vector3 direction = (shellNavMeshEnemy.transform.position - selected.transform.position).normalized;
        yield return StartCoroutine(LaserFrom(selected, direction));
    }

    private IEnumerator LaserFrom(BossBarrel barrel, Vector3 direction)
    {
        if (barrel.IsFiring) yield break;
        barrel.IsFiring = true;

        LineRenderer selected = barrel.LineRenderer;
        // Reset
        selected.widthMultiplier = 0;

        // Show
        selected.enabled = true;

        // Set Positions
        selected.positionCount = 2;
        Ray ray;

        // Grow Laser Width
        while (selected.widthMultiplier < maxLaserWidth)
        {
            // Update Position
            selected.SetPosition(0, shellNavMeshEnemy.transform.position);
            direction = (shellNavMeshEnemy.transform.position - selected.transform.position).normalized;
            ray = new Ray(shellNavMeshEnemy.transform.position, direction);
            selected.SetPosition(1, ray.GetPoint(laserRange));

            // Change Width
            selected.widthMultiplier = Mathf.MoveTowards(selected.widthMultiplier, maxLaserWidth, Time.deltaTime * laserGrowSpeed);

            yield return null;
        }

        // Here we would Enable Damage
        // Continously change position
        for (float t = 0; t < laserStayDuration; t += Time.deltaTime)
        {
            // Update Position
            selected.SetPosition(0, shellNavMeshEnemy.transform.position);
            direction = (shellNavMeshEnemy.transform.position - selected.transform.position).normalized;
            ray = new Ray(shellNavMeshEnemy.transform.position, direction);
            selected.SetPosition(1, ray.GetPoint(laserRange));

            yield return null;
        }

        // Grow Laser Width
        while (selected.widthMultiplier > 0)
        {
            // Update Position
            selected.SetPosition(0, shellNavMeshEnemy.transform.position);
            ray = new Ray(shellNavMeshEnemy.transform.position, direction);
            selected.SetPosition(1, ray.GetPoint(laserRange));

            // Change Width
            selected.widthMultiplier = Mathf.MoveTowards(selected.widthMultiplier, 0, Time.deltaTime * laserGrowSpeed * laserShrinkSpeedModifier);

            yield return null;
        }

        // Disable Damage

        selected.enabled = false;
        barrel.IsFiring = false;
    }
}
