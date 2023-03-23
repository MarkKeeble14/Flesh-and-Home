using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class BakeOnceColumnsAreGenerated : MonoBehaviour
{
    public static BakeOnceColumnsAreGenerated _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;


        numWaitingOn = FindObjectsOfType<RoomController>().Length;
    }

    private int numWaitingOn;
    [SerializeField] private NavMeshSurface surface;

    public void ColumnsLoaded()
    {
        numWaitingOn--;
        // Debug.Log(numWaitingOn);
        if (numWaitingOn == 0)
        {
            surface.BuildNavMesh();
            // Debug.Log("Buidling Nav Mesh");
        }
    }
}
