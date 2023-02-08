using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemy : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent navMeshAgent;
    private PlayerController p;

    public float DistanceToGround
    {
        get
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity);
            return transform.position.y - hit.point.y;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get reference
        p = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is not set (or null cause of dying) stop execution
        if (!p) return;
        navMeshAgent.SetDestination(p.transform.position);
    }
}
