using System.Collections;
using UnityEngine;

public class FleshPodSpawn : RoomEnemyStateController
{
    [Header("Enemy")]
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxVerticalSpawnForce;
    [SerializeField] private Vector2 minMaxHorizontalSpawnForce;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onSpawnClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private Rigidbody rb;

    public void OnSpawn(Transform target)
    {
        // Add Force
        rb.AddForce(new Vector3(
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y),
            Random.Range(minMaxVerticalSpawnForce.x, minMaxVerticalSpawnForce.y),
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y))
            , ForceMode.Impulse);

        // Audio
        onSpawnClip.PlayOneShot(source);
    }
}
