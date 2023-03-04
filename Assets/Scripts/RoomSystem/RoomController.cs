using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private List<RoomEnemy> roomEnemies = new List<RoomEnemy>();
    private List<FeastableEntity> spawnedFeastables = new List<FeastableEntity>();
    private List<FeastableEntity> enemyFeastables = new List<FeastableEntity>();
    private List<FeastableEntity> totalRoomFeastables = new List<FeastableEntity>();
    private FeastableSpawnPointsController spawnPoints;

    [SerializeField] private Vector2 chanceToSpawnFeastableChunk = new Vector2(1, 2);
    [SerializeField] private Vector2 minMaxSecondsBetweenChunksSpawnAttempt = new Vector2(15, 30);
    [SerializeField] private FeastableEntity feastableChunk;
    [SerializeField] private bool shouldSpawnFeastableChunks = true;

    [SerializeField] private Vector2 minMaxFeastablesInBatch = new Vector2(2, 5);
    [SerializeField] private bool useMaxTotalFeastablesSpawned;
    [SerializeField] private int maxTotalFeastablesSpawned = 50;
    [SerializeField] private int feastablesAvailableAtOnce = 10;
    private int totalFeastablesSpawned;

    public List<FeastableEntity> TotalRoomFeastables => totalRoomFeastables;

    public void OnEnter()
    {
        // Debug.Log(name + " Entered For the First Time");

        // Get the spawn points controller for this room
        spawnPoints = GetComponentInChildren<FeastableSpawnPointsController>();

        // Set spawn points array; will find all feastablespawnpoints in the room
        if (spawnPoints != null)
        {
            spawnPoints.SetArray();
        }

        // Get all room content
        enableOnEnter = GetComponentsInChildren<IRoomContent>();

        // Activate all room content
        foreach (IRoomContent roomContent in enableOnEnter)
        {
            roomContent.Activate();
        }

        // 
        roomEnemies.AddRange(GetComponentsInChildren<RoomEnemy>());
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.GetComponent<KillableEntity>().AddAdditionalOnEndAction(() => roomEnemies.Remove(roomEnemy));
            roomEnemy.PlayerIsInRoom = true;
        }

        // When an enemy becomes a corpse add it to the list of feastable entities
        // When an enemy who became a corpse dies, remove it from the list of feastable entities
        foreach (FeastableEntity feastable in GetComponentsInChildren<FeastableEntity>())
        {
            feastable.onBecomeCorpse += delegate
            {
                // Debug.Log(name + " added: " + feastable + " to EnemyFeastables");
                enemyFeastables.Add(feastable);
                totalRoomFeastables.Add(feastable);
            };
            feastable.onCorpseDie += delegate
            {
                // Debug.Log(name + " removed: " + feastable + " from EnemyFeastables");
                enemyFeastables.Remove(feastable);
                totalRoomFeastables.Remove(feastable);
            };
        }

        if (shouldSpawnFeastableChunks)
        {
            // Start Spawning Feastables
            StartCoroutine(SpawnFeastableChunksCycle());
        }
    }

    public void OnReenter()
    {
        // 
        // Debug.Log(name + " Re-entered");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.Aggro();
        }
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.Deaggro();
        }
    }

    private IEnumerator SpawnFeastableChunksCycle()
    {
        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxSecondsBetweenChunksSpawnAttempt));

        // Once spawned the maximum number of feastables for this room, stop spawning feastables
        if (useMaxTotalFeastablesSpawned
            && totalFeastablesSpawned > maxTotalFeastablesSpawned)
        {
            shouldSpawnFeastableChunks = false;
            yield break;
        }

        if (spawnedFeastables.Count < feastablesAvailableAtOnce)
        {
            if (RandomHelper.EvaluateChanceTo(chanceToSpawnFeastableChunk))
            {
                FeastableSpawnPoint spawnPoint = spawnPoints.GetSpawnPoint();

                // Spawn a Batch of feastables
                for (int i = 0; i < RandomHelper.RandomIntInclusive(minMaxFeastablesInBatch); i++)
                {
                    // Debug.Log("Spawning Feastable");
                    FeastableEntity spawned = spawnPoints.Emit(feastableChunk, spawnPoint, true);
                    spawned.name += i;
                    spawnedFeastables.Add(spawned);
                    totalRoomFeastables.Add(spawned);
                    spawned.AddAdditionalOnEndAction(delegate
                    {
                        spawnedFeastables.Remove(spawned);
                        totalRoomFeastables.Remove(spawned);
                    });
                    totalFeastablesSpawned++;
                }
            }
            else
            {
                // Debug.Log("Not Spawning Chunk");
            }
        }

        StartCoroutine(SpawnFeastableChunksCycle());
    }
}
