using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPosition : MonoBehaviour
{
    [SerializeField] private SpawnPosition spawnPosition;
    [SerializeField] private SerializableDictionary<SpawnPosition, SpawnPositionData> spawnPositionDictionary = new SerializableDictionary<SpawnPosition, SpawnPositionData>();
    [SerializeField] private Transform playerObject;
    private PlayerController playerController;

    public void Start()
    {
        playerController = playerObject.GetComponent<PlayerController>();
        ChangePosition(spawnPosition);
    }

    public Vector3 GetSpawnPosition(SpawnPosition spawnPosition)
    {
        return spawnPositionDictionary[spawnPosition].position + Vector3.up / 2;
    }

    public void ChangePosition(SpawnPosition spawnPosition)
    {
        SpawnPositionData data = spawnPositionDictionary[spawnPosition];
        playerController.CancelVelocity();

        Vector3 parentPosition = data.position;
        Vector3 playerObjPosition = Vector3.zero + Vector3.up / 2;

        // Debug.Log("Change Spawn Position Called: " + spawnPosition + ", Parent: " + parentPosition + ", Player: " + playerObjPosition);

        transform.position = parentPosition;
        transform.localEulerAngles = data.eulerAngles;
        playerObject.localPosition = playerObjPosition;
        playerObject.localEulerAngles = Vector3.zero;
    }

    public static string GetTeleportLocationString(SpawnPosition position)
    {
        switch (position)
        {
            case SpawnPosition.BOSS_ROOM:
                return "Boss Room";
            case SpawnPosition.HUB:
                return "Hub";
            case SpawnPosition.OVERWORLD:
                return "Overworld Spawn";
            case SpawnPosition.RUIN_START:
                return "Ruins Start";
            case SpawnPosition.UNLOCK1:
                return "First Unlock";
            case SpawnPosition.UNLOCK2:
                return "Second Unlock";
            case SpawnPosition.UNLOCK3:
                return "Third Unlock";
            default:
                throw new UnhandledSwitchCaseException();
        }
    }
}
