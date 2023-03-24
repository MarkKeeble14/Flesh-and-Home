using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVelocityGameEvent : GameEvent
{
    private PlayerController player;
    [SerializeField] private Vector3 velocityToAdd;

    protected override void Activate()
    {
        player.AddVelocity(velocityToAdd);
    }
    private void Start()
    {
        // Get reference
        player = GameManager._Instance.PlayerTransform.GetComponent<PlayerController>();
    }
}
