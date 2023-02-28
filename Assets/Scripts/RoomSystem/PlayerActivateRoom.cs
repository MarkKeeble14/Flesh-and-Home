using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateRoom : MonoBehaviour
{
    [SerializeField] private LayerMask roomTriggerLayer;
    private Dictionary<RoomController, List<Collider>> childTriggersOfRoom = new Dictionary<RoomController, List<Collider>>();
    private List<RoomController> alreadyTouchedTriggers = new List<RoomController>();
    private Dictionary<Collider, RoomController> colliderControllerDictionary = new Dictionary<Collider, RoomController>();
    private RoomController currentRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, roomTriggerLayer)) return;

        // if we've already collided with this trigger, then it will be stored in the colliderControllerDictionary
        // in this case, we grab from there rather than doing a .GetComponent since it will be cheaper to do a dictionary lookup than a .GetComponent
        RoomController roomTrigger;
        if (colliderControllerDictionary.ContainsKey(other))
        {
            roomTrigger = colliderControllerDictionary[other];
        }
        else
        {
            roomTrigger = other.GetComponent<RoomController>();
        }

        // if entering room for the first time, call on enter, otherwise call on re-enter
        if (alreadyTouchedTriggers.Contains(roomTrigger))
        {
            // if the list that keeps track of child triggers does not yet contain this collider, we add it to that list so that next time it is 
            if (!childTriggersOfRoom[roomTrigger].Contains(other))
            {
                childTriggersOfRoom[roomTrigger].Add(other);
                // Debug.Log(childTriggersOfRoom[roomTrigger].Count);
            }

            if (currentRoom != roomTrigger)
            {
                currentRoom.OnExit();
                roomTrigger.OnReenter();
            }
            currentRoom = roomTrigger;
        }
        else
        {
            roomTrigger.OnEnter();

            // if the current room is not the room we are stepping into, we have changed rooms
            if (currentRoom != null
                && currentRoom != roomTrigger)
            {
                currentRoom.OnExit();
            }
            // set the new current room
            currentRoom = roomTrigger;

            alreadyTouchedTriggers.Add(roomTrigger);

            childTriggersOfRoom.Add(roomTrigger, new List<Collider>());
            childTriggersOfRoom[roomTrigger].Add(other);
            colliderControllerDictionary.Add(other, roomTrigger);
            // Debug.Log(childTriggersOfRoom[roomTrigger].Count);
        }
    }
}
