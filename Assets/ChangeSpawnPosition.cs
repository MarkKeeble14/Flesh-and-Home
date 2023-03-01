using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPosition : MonoBehaviour
{
    [SerializeField] private SpawnPosition spawnPosition;
    [SerializeField] private SerializableDictionary<SpawnPosition, SpawnPositionData> spawnPositionDictionary = new SerializableDictionary<SpawnPosition, SpawnPositionData>();

    public void Start()
    {
        SpawnPositionData data = spawnPositionDictionary[spawnPosition];
        transform.position = data.position;
        transform.localEulerAngles = data.eulerAngles;
    }

    public enum SpawnPosition
    {
        RUIN_START,
        HUB,
        BOSS_ROOM
    }

    [System.Serializable]
    public struct SpawnPositionData
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }
}
