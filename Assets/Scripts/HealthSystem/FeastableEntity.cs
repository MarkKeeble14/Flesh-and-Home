using System;
using System.Collections.Generic;
using UnityEngine;

public class FeastableEntity : KillableEntity
{
    [SerializeField] private KillableEntity fleshyTraveller;
    [SerializeField] private float travellerHP;

    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask feastingEnemyLayer;

    protected override void OnEnd()
    {
        base.OnEnd();

        FeastingEnemy target = FindNearestFeastingEnemy();
        KillableEntity spawned = Instantiate(fleshyTraveller, new Vector3(transform.position.x, TransformHelper.FindGroundPoint(transform), transform.position.z), Quaternion.identity);

        if (target != null)
        {
            spawned.MaxHealth = travellerHP;
            spawned.ResetHealth();
            spawned.transform.localScale = transform.localScale;

            EnemyMovement spawnedMovement = spawned.GetComponent<EnemyMovement>();

            // Debug.Log(name + ", Target: " + target);

            // Move to Enemy
            spawnedMovement.OverrideTarget(target.transform, target.transform.localScale.x + .5f, true, false,
                delegate
                {
                    if (target.Health.IsDead)
                    {
                        // Debug.Log("Failure: " + target);
                        Destroy(spawned.gameObject);
                        Destroy(gameObject);

                    }
                    else
                    {
                        // Debug.Log("Success: " + target);
                        target.FeastOnEntity(this);
                        Destroy(spawned.gameObject);
                        Destroy(gameObject);
                    }
                }, delegate
                {
                    // Debug.Log("Failure");
                    Destroy(spawned.gameObject);
                    Destroy(gameObject);
                });
        }
        else
        {
            Destroy(spawned.gameObject, .25f);
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
