using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserArmEnemy : BasicEnemy
{
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
}
