using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateRoom : MonoBehaviour
{
    [SerializeField] private LayerMask roomTriggerLayer;
    private Dictionary<Collider, RoomController> alreadyTouchedTriggers = new Dictionary<Collider, RoomController>();
    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, roomTriggerLayer)) return;
        if (alreadyTouchedTriggers.ContainsKey(other)) return;
        RoomController roomTrigger = other.GetComponent<RoomController>();
        roomTrigger.OnEnter();
        alreadyTouchedTriggers.Add(other, roomTrigger);
    }
}
