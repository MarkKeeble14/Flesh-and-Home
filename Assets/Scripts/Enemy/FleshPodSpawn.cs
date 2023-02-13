using System.Collections;
using UnityEngine;

public class FleshPodSpawn : BasicEnemy
{
    [Header("Enemy")]
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxVerticalSpawnForce;
    [SerializeField] private Vector2 minMaxHorizontalSpawnForce;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onSpawnClip;

    private void Update()
    {
        // Can't move can't attack? Might not be ideal
        if (!Movement.Move) return;

        // If this enemy can Attack, call a random attack available to it
        // if (Attack.CanAttack(Movement.Target))
        // {
        // Debug.Log("Calling Attack");
        StartCoroutine(Attack.StartAttack(Movement.Target));
        // }
    }

    public void OnSpawn(Transform target)
    {
        // Add Force
        RB.AddForce(new Vector3(
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y),
            Random.Range(minMaxVerticalSpawnForce.x, minMaxVerticalSpawnForce.y),
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y))
            , ForceMode.Impulse);

        // Audio
        onSpawnClip.PlayOneShot(source);
    }
}
