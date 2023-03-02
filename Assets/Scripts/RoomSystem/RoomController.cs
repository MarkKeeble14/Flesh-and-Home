using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    [SerializeField] private CinemachineVirtualCamera mainCam; //normal camera
    [SerializeField] private CinemachineVirtualCamera virtualCam; //pan camera

    [SerializeField] private Transform roomAbove;
    
    private bool normalCamera = true;
    public float rotationTime = 2.0f;

    private Transform originalLookAt;
    private bool isRotating = false;
    

    public void OnEnter()
    {
        Debug.Log(name + " Entered For the First Time");

        if (name == "HubRoom")
        {
            if (normalCamera)
            {
                mainCam.Priority = 9;
                virtualCam.Priority = 10;
                StartCoroutine(RotateCameraUp());
            }
            else
            {
                mainCam.Priority = 10;
                virtualCam.Priority = 9;
            }

            normalCamera = !normalCamera;
        }

        // Get the spawn points controller for this room
        spawnPoints = GetComponentInChildren<FeastableSpawnPointsController>();

        // Set spawn points array; will find all feastablespawnpoints in the room
        spawnPoints.SetArray();

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
    IEnumerator RotateCameraUp()
    {
        isRotating = true;
        
        Quaternion originalRotation = virtualCam.transform.rotation;
        Vector3 lookUp = new Vector3(0, 0, 1000);
        Vector3 targetPosition = roomAbove.position + lookUp;
        
        // Look at the target position
        virtualCam.LookAt = roomAbove;
        
        // Rotate towards the target position
        float elapsedTime = 0.0f;
        while (elapsedTime < rotationTime)
        {
            //Quaternion.LookRotation(targetPosition - virtualCam.transform.position)
            virtualCam.transform.rotation = Quaternion.Slerp(originalRotation, Quaternion.LookRotation(targetPosition - virtualCam.transform.position), elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Reset the camera's LookAt
        virtualCam.LookAt = originalLookAt;
        
        
        
        isRotating = false;

        mainCam.Priority = 10;
        virtualCam.Priority = 9;
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
