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

    public FeastableEntity Emit(FeastableEntity toEmit, bool useRandomOffset)
    {
        FeastableSpawnPoint spawnPoint = GetSpawnPoint();
        return Emit(toEmit, spawnPoint, useRandomOffset);
    }

    public FeastableEntity Emit(FeastableEntity toEmit, FeastableSpawnPoint spawnPoint, bool useRandomOffset)
    {
        return spawnPoint.Spawn(toEmit, useRandomOffset);
    }
}
