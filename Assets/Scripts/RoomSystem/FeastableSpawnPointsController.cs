using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeastableSpawnPointsController : MonoBehaviour
{
    private FeastableSpawnPoint[] spawnPoints;

    public void SetArray()
    {
        spawnPoints = GetComponentsInChildren<FeastableSpawnPoint>();
    }


    public FeastableSpawnPoint GetSpawnPoint()
    {
        return RandomHelper.GetRandomFromArray(spawnPoints);
    }
}
