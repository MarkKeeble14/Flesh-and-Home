using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeastableSpawnPointsController : MonoBehaviour
{
    private FeastableSpawnPoint[] spawnPoints;

    public void SetArray()
    {
        spawnPoints = GetComponentsInChildren<FeastableSpawnPoint>(true);
    }

    public FeastableSpawnPoint GetSpawnPoint()
    {
        return RandomHelper.GetRandomFromArray(spawnPoints);
    }

    public GameObject Emit(GameObject toEmit, bool useRandomOffset)
    {
        FeastableSpawnPoint spawnPoint = GetSpawnPoint();
        return Emit(toEmit, spawnPoint, useRandomOffset);
    }

    public GameObject Emit(GameObject toEmit, FeastableSpawnPoint spawnPoint, bool useRandomOffset)
    {
        return spawnPoint.Spawn(toEmit, useRandomOffset);
    }
}
