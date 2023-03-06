using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTowardsTarget : EnemyMovement
{
    [SerializeField] private float speed;

    [SerializeField] protected bool rotateTowardsMovementDirection;

    // Update is called once per frame
    void Update()
    {
        // Set Audio Source to be enabled/disabled based off of whether this enemy is moving or not
        movementSource.enabled = target && AllowMove;

        // If the player is not set (or null cause of dying) stop execution
        if (!target) return;

        // Don't allow move if not supposed to move
        if (!AllowMove) return;

        if (rotateTowardsMovementDirection)
        {
            transform.LookAt((transform.position - target.transform.position).normalized);
        }

        // Move to target
        transform.position += (target.transform.position - transform.position).normalized * speed * Time.deltaTime;
    }

    public override void SetSpeed(float f)
    {
        speed = f;
    }

    public override float GetSpeed()
    {
        return speed;
    }
}
