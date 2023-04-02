using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForNullEventTriggerer : EventTriggerer
{
    [SerializeField] private GameObject[] waitFor;

    protected override bool Condition
    {
        get
        {
            foreach (GameObject obj in waitFor)
            {
                if (obj != null) return false;
            }
            return true;
        }
    }
}


