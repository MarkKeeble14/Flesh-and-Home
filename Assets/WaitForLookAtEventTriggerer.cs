using UnityEngine;

public class WaitForLookAtEventTriggerer : EventTriggerer
{
    [SerializeField] private Transform t2;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask canHit;

    protected override bool Condition
    {
        get
        {
            RaycastHit hit;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, canHit);

            if (hit.transform == null) return false;
            // Debug.Log(hit.transform.gameObject);

            return hit.transform == t2;
        }
    }
}
