using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private List<RoomEnemy> roomEnemies = new List<RoomEnemy>();

    private List<ParticleSystem> airParticles = new List<ParticleSystem>();

    private void Awake()
    {
        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            airParticles.Add(particleSystem);
            particleSystem.Stop();
        }
    }

    private void ToggleParticles(bool trueIsOn)
    {
        foreach (ParticleSystem particleSystem in airParticles)
        {
            if (trueIsOn)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Stop();
            }
        }
    }

    public void OnEnter()
    {
        // Debug.Log(name + " Entered For the First Time");
        ToggleParticles(true);

        // Get all room content
        enableOnEnter = GetComponentsInChildren<IRoomContent>();

        // Activate all room content
        foreach (IRoomContent roomContent in enableOnEnter)
        {
            roomContent.Activate();
        }

        // 
        roomEnemies.AddRange(GetComponentsInChildren<RoomEnemy>());
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.GetComponent<KillableEntity>().AddAdditionalOnEndAction(() => roomEnemies.Remove(roomEnemy));
            roomEnemy.PlayerIsInRoom = true;
        }
    }

    public void OnReenter()
    {
        // 
        // Debug.Log(name + " Re-entered");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerEnterRoom();
        }

        ToggleParticles(true);
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");

        /*
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerLeaveRoom();
        */

        ToggleParticles(false);
    }
}
