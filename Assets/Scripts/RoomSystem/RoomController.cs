using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;

    [SerializeField] private Vector2 chanceToSpawnFeastableChunk = new Vector2(1, 2);
    [SerializeField] private Vector2 minMaxSecondsBetweenChunksSpawnAttempt = new Vector2(15, 30);
    [SerializeField] private FeastableEntity feastableChunk;
    [SerializeField] private int maxFeastablesSpawned;
    [SerializeField] private bool shouldSpawnFeastableChunks = true;
    private List<FeastableEntity> spawnedFeastables = new List<FeastableEntity>();
    private FeastableSpawnPointsController spawnPoints;

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
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");
    }

    private IEnumerator SpawnFeastableChunksCycle()
    {
        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxSecondsBetweenChunksSpawnAttempt));

        if (spawnedFeastables.Count < maxFeastablesSpawned)
        {
            if (RandomHelper.EvaluateChanceTo(chanceToSpawnFeastableChunk))
            {
                // Debug.Log("Spawning Chunk");
                FeastableSpawnPoint spawnPoint = spawnPoints.GetSpawnPoint();
                FeastableEntity spawned = Instantiate(feastableChunk, spawnPoint.transform.position, Quaternion.identity);
                spawnedFeastables.Add(spawned);
                spawned.AddAdditionalOnEndAction(() => spawnedFeastables.Remove(spawned));
            }
            else
            {
                // Debug.Log("Not Spawning Chunk");
            }
        }

        StartCoroutine(SpawnFeastableChunksCycle());
    }
}
