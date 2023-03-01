using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private bool isDisabledForAttack;
    private bool move = true;
    public bool AllowMove => move && !isDisabledForAttack;

    protected Transform target;
    [SerializeField] private bool playerIsTarget;
    [SerializeField] protected LayerMask isGround;

    [Header("Audio")]
    [SerializeField] protected AudioSource movementSource;

    protected bool overrideTarget;
    protected Transform overridenTarget;

    public Transform Target => target;


    private float GetDistanceToTarget(bool ignoreY)
    {
        if (ignoreY)
        {
            if (overrideTarget)
            {
                return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(overridenTarget.position.x, 0, overridenTarget.position.z));
            }
            else
            {
                return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.position.x, 0, target.position.z));
            }
        }
        else
        {
            if (overrideTarget)
            {
                return Vector3.Distance(transform.position, overridenTarget.position);
            }
            else
            {
                return Vector3.Distance(transform.position, target.position);
            }
        }
    }

    public float DistanceToGround
    {
        get
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, isGround);
            return transform.position.y - hit.point.y;
        }
    }

    private void Start()
    {
        if (playerIsTarget && target == null)
            target = GameManager._Instance.PlayerTransform;
    }

    public IEnumerator GoToOverridenTarget(Transform target, float maxAcceptableDistanceFromTarget, bool ignoreY, bool endOverrideOnReachTarget, bool destroyOnReachTarget, Action otherOnReachTarget)
    {
        // Debug.Log(name + ": Begin Override Target");

        // Set variables
        overrideTarget = true;
        overridenTarget = target;

        // Wait until we reach specified position
        while (GetDistanceToTarget(ignoreY) > maxAcceptableDistanceFromTarget)
        {
            // Debug.Log(name + " Position = " + transform.position + ", Target Position = " + target.position + ", " + " - Distance to Target = " + GetDistanceToTarget(ignoreY));
            if (target == null)
            {
                // Debug.Log("Target became null; possible due to being destroyed");
                // if meant to stop override target on end, do so
                overrideTarget = !endOverrideOnReachTarget;

                yield break;
            }
            // Debug.Log(name + " Moving to Target: " + transform.position + ", " + target.transform.position);
            yield return null;
        }

        // Debug.Log("Reached Target");

        // if meant to stop override target on end, do so
        overrideTarget = !endOverrideOnReachTarget;

        // Call whatever functions passed in once reached target
        otherOnReachTarget?.Invoke();

        // if meant to destroy on end, do so
        if (destroyOnReachTarget)
            Destroy(gameObject);
    }

    public void SetDisabledForAttack(bool v)
    {
        isDisabledForAttack = v;
    }

    public void SetMove(bool v)
    {
        move = v;
    }
}
