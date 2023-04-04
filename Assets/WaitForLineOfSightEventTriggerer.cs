using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForLineOfSightEventTriggerer : EventTriggerer
{
    [SerializeField] private Transform t1;
    [SerializeField] private Transform[] t2;

    [SerializeField] private LayerMask canHit;
    [SerializeField] private bool mustHitAllTargets;

    protected override bool Condition
    {
        get
        {
            if (t1 == null)
            {
                Destroy(gameObject);
                return false;
            }

            if (mustHitAllTargets)
            {
                foreach (Transform target in t2)
                {
                    RaycastHit hit;
                    Physics.Raycast(t1.position, target.position - t1.position, out hit, Mathf.Infinity, canHit);
                    if (hit.transform != target) return false;
                }
                return true;
            }
            else
            {
                foreach (Transform target in t2)
                {
                    RaycastHit hit;
                    Physics.Raycast(t1.position, target.position - t1.position, out hit, Mathf.Infinity, canHit);
                    if (hit.transform == target) return true;
                }
                return false;
            }
        }
    }
}
