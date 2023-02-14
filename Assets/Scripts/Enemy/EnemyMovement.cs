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
    protected Vector3 overridenTargetPosition;

    public Transform Target => target;

    public float DistanceToTarget => Vector3.Distance(transform.position, overrideTarget ? overridenTargetPosition : target.position);

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


    public IEnumerator GoToOverridenTarget(Vector3 position, float fudgeDistance, bool ignoreY, bool endOverrideOnReachTarget, bool destroyOnReachTarget, Action otherOnReachTarget)
    {
        // Set variables
        overrideTarget = true;
        overridenTargetPosition = position;

        Vector3 targetPos;
        // Wait until we reach specified position
        while (DistanceToTarget > fudgeDistance)
        {
            targetPos = (ignoreY ? transform.position - (Vector3.up * transform.position.y) : transform.position);
            // Debug.Log(name + " Moving to Target: " + transform.position + ", " + position);
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
