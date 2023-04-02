using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HurlAttack : Attack
{
    [SerializeField] private float waitTime;
    [SerializeField] private float launchForce;
    [SerializeField] private AudioClipContainer onPrime;
    private RoomEnemyStateController roomEnemyStateController;
    private Transform parent;

    [SerializeField] private HurlAttackCollisionEvents onHurlCollisions;

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        roomEnemyStateController = GetComponentInParent<RoomEnemyStateController>();
        parent = roomEnemyStateController.transform;
        roomEnemyStateController.enabled = false;
        EnemyMovement movement = parent.GetComponent<EnemyMovement>();
        movement.SetMove(false);
        if (movement is NavMeshEnemy)
        {
            ((NavMeshEnemy)movement).DisableNavMeshAgent();
        }

        yield return new WaitForSeconds(waitTime);

        if (movement is NavMeshEnemy)
        {
            ((NavMeshEnemy)movement).DisableNavMeshAgent();
        }

        Rigidbody rb = parent.GetComponent<Rigidbody>();
        Collider col = parent.GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = true;
        rb.isKinematic = false;
        Vector3 addedForce = launchForce * (target.position - rb.transform.position).normalized;
        rb.AddForce(addedForce);
        onPrime.PlayOneShot(source);
        onHurlCollisions.primed = true;
    }
}
