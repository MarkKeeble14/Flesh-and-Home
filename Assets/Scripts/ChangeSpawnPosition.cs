using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPosition : MonoBehaviour
{
    [SerializeField] private SpawnPosition spawnPosition;
    [SerializeField] private SerializableDictionary<SpawnPosition, SpawnPositionData> spawnPositionDictionary = new SerializableDictionary<SpawnPosition, SpawnPositionData>();
    [SerializeField] private Transform playerObject;

    public void Start()
    {
        ChangePosition(spawnPosition);
    }

    public void ChangePosition(SpawnPosition spawnPosition)
    {
        SpawnPositionData data = spawnPositionDictionary[spawnPosition];
        transform.position = data.position;
        playerObject.localPosition = Vector3.zero + Vector3.up / 2;
        transform.localEulerAngles = data.eulerAngles;
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
