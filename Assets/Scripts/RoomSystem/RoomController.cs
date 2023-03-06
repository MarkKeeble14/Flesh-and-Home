using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private List<RoomEnemy> roomEnemies = new List<RoomEnemy>();

    public void OnEnter()
    {
        // Debug.Log(name + " Entered For the First Time");
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
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerLeaveRoom();
        }
    }
}
