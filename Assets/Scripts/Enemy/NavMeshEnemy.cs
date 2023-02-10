using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NavMeshEnemy : EnemyMovement
{
    protected NavMeshAgent navMeshAgent;

    public bool IsActive { get; protected set; }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set Movement source to be active if moving basically
        movementSource.enabled = IsActive && Target && navMeshAgent.isOnNavMesh && Move;

        // Nav Mesh Agent has not become active yet
        if (!IsActive)
        {
            // Debug.Log("Not Active");
            return;
        };

        // If the player is not set (or null cause of dying) stop execution
        if (!Target)
        {
            // Debug.Log("No Target Set");
            return;
        };

        // The Nav Mesh Agent is not on the NavMesh for some reason
        // if (!navMeshAgent.isOnNavMesh) return;

        // Don't allow move if not supposed to move
        if (!Move)
        {
            // Debug.Log("Not Supposed to Move");
            return;
        };

        // Stop if not supposed to move so other things can move this object
        navMeshAgent.isStopped = !Move;

        // Move to target
        navMeshAgent.SetDestination(Target.transform.position);
    }

    public void DisableNavMeshAgent()
    {
        navMeshAgent.enabled = false;
        IsActive = false;
    }

    public void EnableNavMeshAgent()
    {
        navMeshAgent.enabled = true;
        IsActive = true;
    }
}
