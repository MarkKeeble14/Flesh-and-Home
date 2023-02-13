using System.Collections.Generic;
using UnityEngine.AI;

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
        movementSource.enabled = IsActive && target && navMeshAgent.isOnNavMesh && Move;

        // Nav Mesh Agent has not become active yet
        if (!IsActive)
        {
            // Debug.Log("Not Active");
            return;
        };

        // The Nav Mesh Agent is not on the NavMesh for some reason
        // if (!navMeshAgent.isOnNavMesh) return;

        // Stop if not supposed to move so other things can move this object
        navMeshAgent.isStopped = !Move;

        // Don't allow move if not supposed to move
        if (!Move)
        {
            // Debug.Log("Not Supposed to Move");
            return;
        };

        // if we are meant to go to a specified target position rather than a transfom
        if (overrideTarget)
        {
            // Move to position
            navMeshAgent.SetDestination(overridenTargetPosition);
            return;
        }

        // We are not overriding target position, just go to target if set
        // If the player is not set (or null cause of dying) stop execution
        if (!target)
        {
            // Debug.Log("No Target Set");
            return;
        };

        // Move to target
        navMeshAgent.SetDestination(target.transform.position);
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
