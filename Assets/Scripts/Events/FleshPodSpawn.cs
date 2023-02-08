using UnityEngine;
using UnityEngine.AI;

public class FleshPodSpawn : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 minMaxVerticalSpawnForce;
    [SerializeField] private Vector2 minMaxHorizontalSpawnForce;
    [SerializeField] private LayerMask ground;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshEnemy movement;
    [SerializeField] private NavMeshAgent agent;

    public void OnSpawn()
    {
        // Add Force
        rb.AddForce(new Vector3(
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y),
            Random.Range(minMaxVerticalSpawnForce.x, minMaxVerticalSpawnForce.y),
            Random.Range(minMaxHorizontalSpawnForce.x, minMaxHorizontalSpawnForce.y))
            , ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMaskHelper.IsInLayerMask(collision.gameObject, ground))
        {
            agent.enabled = true;
            movement.enabled = true;
        }
    }
}
