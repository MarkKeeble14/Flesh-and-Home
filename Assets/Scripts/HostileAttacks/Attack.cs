using System;
using System.Collections;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private bool hasTimeBetweenAttacks;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool disableMoveWhileBetweenAttacks;
    [SerializeField] private bool disableMovementWhileAttacking;
    [SerializeField] private bool followWhileAttacking;
    [SerializeField] private bool rotateWhileAttacking;

    [Header("Range Settings")]
    [SerializeField] private bool hasMaxRange;
    [SerializeField] private float maxRange;

    [SerializeField] private bool removeFromPoolWhenActive;
    public bool RemoveFromPoolWhenActive => removeFromPoolWhenActive;

    private bool betweenAttacks;

    private bool isDisablingMovement;
    public bool IsDisablingMovement => isDisablingMovement;

    private bool startedAttacking;
    public bool StartedAttacking
    {
        get
        {
            return startedAttacking;
        }
        protected set
        {
            startedAttacking = value;
        }
    }

    protected bool currentlyAttacking;


    public bool HasMaxRange { get => hasMaxRange; set => hasMaxRange = value; }
    public float MaxRange { get => maxRange; set => maxRange = value; }

    [Header("References")]
    [Header("Audio")]
    [SerializeField] protected AudioSource source;
    [SerializeField] private AudioClipContainer onStartClip;
    [SerializeField] private AudioClipContainer onEndClip;
    protected bool executionInterrupted;

    public IEnumerator StartAttack(Transform target, AttackingEnemy enemy)
    {
        startedAttacking = true;

        // Add attack to enemy tracker
        enemy.AddCurrentAttack(this);

        // Debug.Log("Starting Attack - 3");

        // Audio
        onStartClip.PlayOneShot(source);

        // Actually call attack
        StartCoroutine(CallAttack(target));

        // 
        while (currentlyAttacking)
        {
            // Debug.Log(name + ", Currently Attacking: " + currentlyAttacking);
            // Set rotation if desired
            if (rotateWhileAttacking)
            {
                Vector3 lookPos = target.position - enemy.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                enemy.transform.rotation = rotation;
            }

            // if we are supposed to disable movement while attacking
            if (disableMovementWhileAttacking)
            {
                isDisablingMovement = true;
            }
            else if (followWhileAttacking)
            {
                isDisablingMovement = WithinRange(target);
            }
            yield return null;
        }

        executionInterrupted = false;

        // Debug.Log("After Attack - 4");

        // Audio
        onEndClip.PlayOneShot(source);

        // Not actively attacking anymore
        startedAttacking = false;

        // Wait some time if intended to
        if (hasTimeBetweenAttacks)
        {
            isDisablingMovement = disableMoveWhileBetweenAttacks;
            betweenAttacks = true;
            yield return new WaitForSeconds(timeBetweenAttacks);
            betweenAttacks = false;
        }

        // Remove attack from enemy tracker
        enemy.RemoveCurrentAttack(this);

        // Re-enable movement
        isDisablingMovement = false;
    }

    private IEnumerator CallAttack(Transform target)
    {
        // Actually call attack
        currentlyAttacking = true;

        // Debug.Log("Currently Attacking now True");

        yield return StartCoroutine(ExecuteAttack(target));

        // Debug.Log("Currently Attacking now False");

        currentlyAttacking = false;
    }

    protected abstract IEnumerator ExecuteAttack(Transform target);

    public virtual bool CanAttack(Transform target)
    {
        // Debug.Log("1st Condition: " + hasMaxRange + ", " + !WithinRange(target) + ", Result: " + (hasMaxRange && !WithinRange(target))
        //    + " | 2nd Condition: " + currentlyAttacking);
        if (hasMaxRange && !WithinRange(target))
        {
            // Debug.Log("Can't Attack: Not in Range");
            return false;
        }
        if (startedAttacking)
        {
            // Debug.Log("Can't Attack: Currently Attacking");
            return false;
        }
        if (betweenAttacks)
        {
            // Debug.Log("Can't Attack: Between Attacks");
            return false;
        }
        return true;
    }

    public virtual bool WithinRange(Transform target)
    {
        if (target == null)
        {
            // Debug.Log("Can't Attack: Target is null");
            return false;
        }
        if (GetDistanceToTransform(target) > maxRange)
        {
            // Debug.Log("Can't Attack: Not in Range");
            return false;
        }
        return true;
    }

    public virtual void Interrupt()
    {
        // Debug.Log(name + ": Interrupted");
        executionInterrupted = true;
    }

    protected float GetDistanceToTransform(Transform target) => Vector3.Distance(transform.position, target.position);
}
