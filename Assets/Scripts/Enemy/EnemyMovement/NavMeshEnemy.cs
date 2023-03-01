using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemy : EnemyMovement
{
    protected NavMeshAgent navMeshAgent;
    [SerializeField] private bool enableOnAwake;

    public bool IsActive { get; protected set; }


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (enableOnAwake)
            EnableNavMeshAgent();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(name + ": " + AllowMove);

        // Nav Mesh Agent has not become active yet
        if (!IsActive)
        {
            movementSource.enabled = false;
            // Debug.Log("Not Active");
            return;
        };

        // The Nav Mesh Agent is not on the NavMesh for some reason
        // if (!navMeshAgent.isOnNavMesh) return;

        // Stop if not supposed to move so other things can move this object
        navMeshAgent.isStopped = !AllowMove;

        // Don't allow move if not supposed to move
        if (!AllowMove)
        {
            movementSource.enabled = false;
            // Debug.Log("Not Supposed to Move");
            return;
        };

        // if we are meant to go to a specified target position rather than a transfom
        if (overrideTarget)
        {
            // Debug.Log("Override Target");

            // Move to position
            navMeshAgent.SetDestination(overridenTarget.position);
            movementSource.enabled = true;
            return;
        }

        // We are not overriding target position, just go to target if set
        // If the player is not set (or null cause of dying) stop execution
        if (!target)
        {
            // Debug.Log("No Target Set");

            movementSource.enabled = false;
            return;
        };


        // Move to target
        navMeshAgent.SetDestination(target.transform.position);
        movementSource.enabled = true;
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
