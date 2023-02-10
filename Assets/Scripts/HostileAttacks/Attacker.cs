using System;
using System.Collections;
using UnityEngine;

public abstract class Attacker : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private bool hasTimeBetweenAttacks;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool enemyCanMoveWhileBetweenAttacks;

    [SerializeField] private bool currentlyAttacking;
    public bool CurrentlyAttacking
    {
        get
        {
            return currentlyAttacking;
        }
        protected set
        {
            currentlyAttacking = value;
        }
    }

    [Header("References")]
    private EnemyMovement movement;
    private bool hasMovement => movement != null;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onStartClip;
    [SerializeField] private AudioClipContainer onEndClip;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        // Try to get a reference to a movement script on this object
        movement = GetComponent<EnemyMovement>();
    }

    public IEnumerator StartAttack(Transform target)
    {
        yield return StartCoroutine(StartAttack(target, null));
    }

    public IEnumerator StartAttack(Transform target, Action onEnd)
    {
        CurrentlyAttacking = true;

        // Audio
        onStartClip.PlayOneShot(source);

        yield return StartCoroutine(Attack(target));

        // Audio
        onEndClip.PlayOneShot(source);

        CurrentlyAttacking = false;

        if (hasTimeBetweenAttacks)
        {
            if (hasMovement)
                movement.Move = enemyCanMoveWhileBetweenAttacks;
            yield return new WaitForSeconds(timeBetweenAttacks);
            if (hasMovement)
                movement.Move = true;
        }

        onEnd?.Invoke();
    }

    protected abstract IEnumerator Attack(Transform target);

    public virtual bool CanAttack(Transform target)
    {
        if (CurrentlyAttacking) return false;
        return true;
    }

    protected float GetDistanceToTransform(Transform target) => Vector3.Distance(transform.position, target.position);
}
