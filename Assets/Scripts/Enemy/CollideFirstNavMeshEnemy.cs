using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CollideFirstNavMeshEnemy : NavMeshEnemy
{
    [Header("Collision Settings")]
    [SerializeField] private float timeAfterGroundCollisionToMove;
    [SerializeField] private LayerMask ground;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onCollideWithGroundClip;
    [SerializeField] private AudioSource source;

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMaskHelper.IsInLayerMask(collision.gameObject, ground))
        {
            // Audio
            onCollideWithGroundClip.PlayOneShot(source);

            StartCoroutine(StartMove());
        }
    }

    private IEnumerator StartMove()
    {
        yield return new WaitForSeconds(timeAfterGroundCollisionToMove);
        navMeshAgent.enabled = true;
        IsActive = true;
    }
}
