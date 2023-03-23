using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnFeastableTraveller : NavMeshEnemy
{
    [SerializeField] private KillableEntity spawned;
    [SerializeField] private LayerMask feastingEnemyLayer;
    [SerializeField] private float detectionRange;

    private void Start()
    {
        // Find target
        FeastingEnemy target = FindNearestFeastingEnemy();

        if (target != null)
        {
            spawned.transform.localScale = transform.localScale;

            // Debug.Log(name + ", Target: " + target);

            SetMove(true);

            // Move to Enemy
            OverrideTarget(target.transform, target.transform.localScale.x / 2 + transform.localScale.x / 2 + .25f, true, false,
                delegate
                {
                    if (target.Health.IsDead)
                    {
                        // Debug.Log("Failure: " + target);
                        Destroy(gameObject);
                    }
                    else
                    {
                        // Debug.Log("Success: " + target);
                        target.FeastOnEntity(spawned);
                        Destroy(gameObject);
                    }
                }, delegate
                {
                    // Debug.Log("Failure");
                    Destroy(gameObject);
                });
        }
        else
        {
            // Debug.Log("No Target; Destroying");
            Destroy(gameObject);
        }
    }

    private FeastingEnemy FindNearestFeastingEnemy()
    {
        // TODO
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRange, feastingEnemyLayer);
        List<Collider> candidateEnemies = new List<Collider>();
        foreach (Collider col in enemiesInRange)
        {
            if (col.gameObject.Equals(gameObject)) continue;
            candidateEnemies.Add(col);
        }
        // Debug.Log("There are " + candidateEnemies.Count + " candidates in range of " + name);
        if (candidateEnemies.Count == 0)
        {
            // Debug.Log("No Candidates around " + name + " - Returning Null");
            return null;
        }
        Transform chosenFeastingEnemy = TransformHelper.GetClosestTransformToTransform(transform, candidateEnemies);
        // Debug.Log("Chosen Candidate: " + chosenFeastingEnemy);
        return chosenFeastingEnemy.GetComponent<FeastingEnemy>();
    }
}
