using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private List<RoomEnemy> roomEnemies = new List<RoomEnemy>();

    private List<ParticleSystem> airParticles = new List<ParticleSystem>();

    private ColumnCongifuration[] columnConfigurations;

    private FeastableSpawnPointsController feastableSpawnPointsController;

    [Header("Can Spawn Feastables")]
    [SerializeField] private int maxSpawned = 2;
    [SerializeField] private float timeBetweenSpawnAttempts = 10f;
    [SerializeField] private Vector2 chanceToSpawn = new Vector2(1, 3);
    [SerializeField] private KillableEntity feastableSpawn;
    private Coroutine spawnLoop;

    private IEnumerator TrySpawnLoop()
    {
        // Debug.Log(name + ": Try Spawn Loop");
        if (feastableSpawnPointsController == null)
        {
            // Debug.Log(name + ": Try Spawn Loop: No Spawn Points");
            yield break;
        }

        if (maxSpawned <= 0)
        {
            // Debug.Log(name + ": Try Spawn Loop: No More Spawns");
            yield break;
        }
        yield return new WaitForSeconds(timeBetweenSpawnAttempts);

        if (RandomHelper.EvaluateChanceTo(chanceToSpawn))
        {
            // Debug.Log(name + ": Try Spawn Loop: Spawn");
            maxSpawned--;
            feastableSpawnPointsController.Emit(feastableSpawn.gameObject, false);
        }

        StartCoroutine(TrySpawnLoop());
    }

    private void Awake()
    {
        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            airParticles.Add(particleSystem);
            particleSystem.Stop();
        }

        feastableSpawnPointsController = GetComponentInChildren<FeastableSpawnPointsController>();
        if (feastableSpawnPointsController)
            feastableSpawnPointsController.SetArray();
        columnConfigurations = GetComponentsInChildren<ColumnCongifuration>(true);
    }

    private void Start()
    {
        // Set a column configuration to be active
        if (columnConfigurations.Length > 0)
        {
            columnConfigurations[RandomHelper.RandomIntExclusive(0, columnConfigurations.Length)].gameObject.SetActive(true);
        }
        BakeOnceColumnsAreGenerated._Instance.ColumnsLoaded();
    }

    private void ToggleParticles(bool trueIsOn)
    {
        foreach (ParticleSystem particleSystem in airParticles)
        {
            if (trueIsOn)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Stop();
            }
        }
    }

    public void OnEnter()
    {
        // Debug.Log(name + " Entered For the First Time");
        ToggleParticles(true);

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

        // Debug.Log(name + ": Try Spawn Loop Started");
        spawnLoop = StartCoroutine(TrySpawnLoop());
    }

    public void OnReenter()
    {
        // 
        // Debug.Log(name + " Re-entered");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerEnterRoom();
        }

        ToggleParticles(true);

        // Debug.Log(name + ": Try Spawn Loop Started");
        spawnLoop = StartCoroutine(TrySpawnLoop());
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");

        /*
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerLeaveRoom();
        */

        ToggleParticles(false);

        if (spawnLoop != null)
            StopCoroutine(spawnLoop);
        spawnLoop = null;
        // Debug.Log(name + ": Try Spawn Loop Stopped");
    }
}
