using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerIntermediary : MonoBehaviour
{
    public UnityEvent actions;
    public void Act()
    {
        actions.Invoke();
    }
}
