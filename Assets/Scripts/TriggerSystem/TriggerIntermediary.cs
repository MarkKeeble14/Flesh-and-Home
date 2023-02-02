using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerIntermediary : MonoBehaviour
{
    [Tooltip("List of functions to call when this object gets triggered.")]
    public UnityEvent actions;
    public void Act()
    {
        actions.Invoke();
    }
}
