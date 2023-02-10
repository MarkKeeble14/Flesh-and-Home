using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : KillableEntity
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    public Rigidbody RB => rb;
    [SerializeField] private EnemyMovement movement;
    public EnemyMovement Movement => movement;
    [SerializeField] private Attacker[] attacks;
    public Attacker Attack => attacks.Length > 0 ? attacks[Random.Range(0, attacks.Length)] : null;
}
