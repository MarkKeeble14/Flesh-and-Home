using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFleshySpawner : MonoBehaviour
{
    [SerializeField] private BossFleshSpawn fleshPrefab;

    [SerializeField] private LayerMask spawnOn;

    [Header("Settings")]
    [SerializeField] private Vector2 minMaxNumPerSpawn;
    [SerializeField] private Vector3 perSpawnOffsetRange;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private Vector2 minMaxScale;
    [SerializeField] private Vector2 minMaxGrowSpeed;
    [SerializeField] private Vector2 minMaxShrinkSpeed;
    [SerializeField] private float heightAllowance = 5f;
    [SerializeField] private bool spawnOnGround = true;

    [Header("Enemy Spawning")]
    [SerializeField] private int maxNumEnemiesCanHaveSpawned;
    [SerializeField] private Vector2 chanceToSpawnEnemyPerFrame;
    [SerializeField] private PercentageMap<EndableEntity> enemyOptions = new PercentageMap<EndableEntity>();
    private List<EndableEntity> spawnedEntities = new List<EndableEntity>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBossFlesh());
    }

    private IEnumerator SpawnBossFlesh()
    {
        // Raycast to find where ground is
        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up * heightAllowance, Vector3.down, out hit, Mathf.Infinity, spawnOn);

        int numToSpawn = (int)Random.Range(minMaxNumPerSpawn.x, minMaxNumPerSpawn.y);
        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 spawnPos = (spawnOnGround ? hit.point : transform.position)
                + new Vector3(
                    Random.Range(-perSpawnOffsetRange.x, perSpawnOffsetRange.x),
                    Random.Range(-perSpawnOffsetRange.y, perSpawnOffsetRange.y),
                    Random.Range(-perSpawnOffsetRange.z, perSpawnOffsetRange.z)
                );
            BossFleshSpawn spawn = Instantiate(fleshPrefab, spawnPos, Quaternion.identity, ClutterSavior._Instance.transform);
            spawn.Set(minMaxScale, minMaxGrowSpeed, minMaxShrinkSpeed);

            // Chance to spawn an enemy 
            if (spawnedEntities.Count < maxNumEnemiesCanHaveSpawned)
            {
                if (Random.value <= chanceToSpawnEnemyPerFrame.x / chanceToSpawnEnemyPerFrame.y)
                {
                    Vector3 groundPos = hit.point;
                    EndableEntity spawned = Instantiate(enemyOptions.GetOption(), transform.position, Quaternion.identity);
                    spawned.AddAdditionalOnEndAction(() =>
                    {
                        // Remove from spawned
                        spawnedEntities.Remove(spawned);
                    });

                    // Add to spawned
                    spawnedEntities.Add(spawned);

                    if (spawned.TryGetComponent(out RoomEnemyStateController enemyStateController))
                    {
                        enemyStateController.Activate();
                        enemyStateController.OverrideToAggroState();
                    }
                }
            }
        }

        yield return new WaitForSeconds(timeBetweenSpawns);

        StartCoroutine(SpawnBossFlesh());
    }
}
