using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemy : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange;
    private bool attacking;

    [Header("References")]
    private PlayerController p;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        // Get reference
        p = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is not set (or null cause of dying) stop execution; alternatively if we're already attacking, shouldn't move nor attack until complete
        if (!p) return;

        if (attacking)
        {
            // In attack range, no need to move more
            navMeshAgent.SetDestination(transform.position);
        }
        else
        {
            // Not in attack range, move towards player
            navMeshAgent.SetDestination(p.transform.position);
        }

        // Control animation
        anim.SetBool("Run Forward", !attacking);

        // Check to see if we're in attack range, if so attack
        if (Vector3.Distance(transform.position, p.transform.position) < attackRange)
        {
            if (!attacking)
                StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        attacking = true;

        // Testing Animator stuff
        anim.SetTrigger("Stab Attack");

        // Temporary just to test
        yield return new WaitForSeconds(2.5f);

        attacking = false;
    }
}
