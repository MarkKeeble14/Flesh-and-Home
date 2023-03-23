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
}
