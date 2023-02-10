using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTestEventInvoker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TriggerIntermediary trg = other.gameObject.GetComponent<TriggerIntermediary>();
        if (trg != null)
        {
            trg.Act();
        }
    }
}
