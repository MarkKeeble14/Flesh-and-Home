using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CollideFirstNavMeshEnemy : NavMeshEnemy
{
    [Header("Collision Settings")]
    [SerializeField] private float timeAfterGroundCollisionToMove;
    [Header("Audio")]
    [SerializeField] private AudioClipContainer onCollideWithGroundClip;
    [SerializeField] private AudioSource source;

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMaskHelper.IsInLayerMask(collision.gameObject, isGround))
        {
            // Audio
            onCollideWithGroundClip.PlayOneShot(source);

            StartCoroutine(StartMove());
        }
    }

    private IEnumerator StartMove()
    {
        yield return new WaitForSeconds(timeAfterGroundCollisionToMove);
        EnableNavMeshAgent();
        SetMove(true);
    }
}
