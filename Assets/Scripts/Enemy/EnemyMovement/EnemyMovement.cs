using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour
{
    private bool isDisabledForAttack;
    private bool move = true;
    public bool AllowMove => move && !isDisabledForAttack;

    protected Transform target;
    public Transform Target => target;
    [SerializeField] private bool playerIsTarget;
    [SerializeField] protected LayerMask isGround;

    [Header("Audio")]
    [SerializeField] protected AudioSource movementSource;

    [SerializeField] private Transform dummyPoint;

    private Stack<TargetData> targets = new Stack<TargetData>();
    private TargetData currentTarget;
    private bool hasReachedTargetPosition;

    private Stack<Transform> availableDummyPoints = new Stack<Transform>();
    private List<Transform> spawnedDummyPoints = new List<Transform>();

    public abstract void SetSpeed(float f);
    public abstract float GetSpeed();

    public struct TargetData
    {
        public Transform target;
        public Action onSuccess;
        public Action onFailure;
        public bool overrideIgnoreY;
        public float overrideAcceptableDistanceBetweenTargetAndMover;
        public bool destroyOnReachTarget;

        public TargetData(Transform target, Action onSuccess, Action onFailure, bool overrideIgnoreY, float overrideAcceptableDistanceBetweenTargetAndMover, bool destroyOnReachTarget)
        {
            this.target = target;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
            this.overrideIgnoreY = overrideIgnoreY;
            this.overrideAcceptableDistanceBetweenTargetAndMover = overrideAcceptableDistanceBetweenTargetAndMover;
            this.destroyOnReachTarget = destroyOnReachTarget;
        }
    }

    private float GetDistanceToTarget(bool ignoreY)
    {
        if (ignoreY)
        {
            return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.position.x, 0, target.position.z));
        }
        else
        {
            return Vector3.Distance(transform.position, target.position);
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

        if (TryGetComponent(out EndableEntity endableEntity))
        {
            endableEntity.AddAdditionalOnEndAction(() => DestroyPoints());
        }
    }

    public void ClearTargets()
    {
        while (targets.Count > 0)
        {
            TargetData popped = targets.Pop();
            popped.onFailure?.Invoke();
            if (popped.destroyOnReachTarget)
            {
                if (spawnedDummyPoints.Contains(popped.target))
                {
                    availableDummyPoints.Push(popped.target);
                }
                else
                {
                    Destroy(popped.target.gameObject);
                }
            }
        }

        target = null;
    }

    private void DestroyPoints()
    {
        foreach (Transform dummyPoint in spawnedDummyPoints)
        {
            Destroy(dummyPoint.gameObject);
        }
    }

    private void NextTarget()
    {
        currentTarget = targets.Pop();
    }

    public void OverrideTarget(Vector3 targetPos, float acceptableDistanceBetweenTargetAndMover, bool ignoreY, Action onSuccess, Action onFailure)
    {
        Transform dummyPoint;
        if (availableDummyPoints.Count == 0)
        {
            dummyPoint = Instantiate(this.dummyPoint, targetPos, Quaternion.identity, ClutterSavior._Instance.transform);
            spawnedDummyPoints.Add(dummyPoint);
        }
        else
        {
            dummyPoint = availableDummyPoints.Pop();
            dummyPoint.position = targetPos;
        }
        OverrideTarget(dummyPoint, acceptableDistanceBetweenTargetAndMover, ignoreY, true, onSuccess, onFailure);
    }

    protected void Update()
    {
        if (hasReachedTargetPosition)
        {
            return;
        }
        else
        {
            if (target == null)
            {
                hasReachedTargetPosition = true;
                currentTarget.onFailure?.Invoke();
                return;
            }
            else
            {
                // Debug.Log(name + ": Distance to Go: " + GetDistanceToTarget(overrideIgnoreY));
                if (GetDistanceToTarget(currentTarget.overrideIgnoreY) <= currentTarget.overrideAcceptableDistanceBetweenTargetAndMover)
                {
                    hasReachedTargetPosition = true;
                    currentTarget.onSuccess?.Invoke();

                    if (currentTarget.destroyOnReachTarget)
                    {
                        if (spawnedDummyPoints.Contains(currentTarget.target))
                        {
                            availableDummyPoints.Push(currentTarget.target);
                        }
                        else
                        {
                            Destroy(currentTarget.target.gameObject);
                        }
                    }

                    if (targets.Count > 0)
                        NextTarget();
                }
            }
        }
    }

    public void OverrideTarget(Transform target, float acceptableDistanceBetweenTargetAndMover, bool ignoreY, bool destroyTargetOnEnd, Action onSuccess, Action onFailure)
    {
        if (target != null)
        {
            targets.Push(currentTarget);
        }

        TargetData newTarget = new TargetData(target, onSuccess, onFailure, ignoreY, acceptableDistanceBetweenTargetAndMover, destroyTargetOnEnd);
        currentTarget = newTarget;
        this.target = currentTarget.target;
        hasReachedTargetPosition = false;
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
