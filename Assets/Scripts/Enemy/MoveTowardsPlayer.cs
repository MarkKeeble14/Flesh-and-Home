using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTowardsPlayer : EnemyMovement
{
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        // Set Audio Source to be enabled/disabled based off of whether this enemy is moving or not
        movementSource.enabled = Target && Move;

        // If the player is not set (or null cause of dying) stop execution
        if (!Target) return;

        // Don't allow move if not supposed to move
        if (!Move) return;

        // Move to target
        transform.position += (Target.transform.position - transform.position).normalized * speed * Time.deltaTime;
    }
}
