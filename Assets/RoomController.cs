using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private bool hasActivated;

    public void OnEnter()
    {
        if (hasActivated) return;
        hasActivated = true;

        // Get all room content
        enableOnEnter = GetComponentsInChildren<IRoomContent>();

        // Activate all room content
        foreach (IRoomContent roomContent in enableOnEnter)
        {
            roomContent.Activate();
        }
    }
}
