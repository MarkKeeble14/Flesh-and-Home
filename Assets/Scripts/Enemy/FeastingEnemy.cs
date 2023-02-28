using System.Collections;
using UnityEngine;

public class FeastingEnemy : MovementBasedEnemy
{
    [Header("Feasting Settings")]
    [SerializeField] private float feastTime;
    [SerializeField] private bool canFeastEnemies;
    [SerializeField] private LayerMask feastable;
    [SerializeField] private float feastableDetectionRange;

    [Tooltip("Additive")]
    [SerializeField] private float onFeastSizeChange;
    [Tooltip("Multiplicative")]
    [SerializeField] private float onFeastHPChange;
    private Collider[] feastablesInRange;
    private bool feastLockedOn;
    private int feastLevel;
    public int FeastLevel => feastLevel;
    private FeastableEntity feasting;

    [SerializeField] private float scaleSpeed = 1f;
    private Vector3 targetScale;

    [SerializeField] private bool disableAttacksWhileFeasting = true;
    private bool isFeasting;

    public override bool DisablingAttacking => disableAttacksWhileFeasting && isFeasting ? true : false;


    protected new void Start()
    {
        base.Start();

        targetScale = transform.localScale;

        StartCoroutine(CheckForFeastables());
        currentState = StartCoroutine(AttackCycle());
    }

    private void Update()
    {
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
        }
    }

    private IEnumerator CheckForFeastables()
    {
        while (true)
        {
            if (feastLockedOn) yield return null;

            feastablesInRange = Physics.OverlapSphere(transform.position, feastableDetectionRange, feastable);
            if (canFeastEnemies && feastablesInRange.Length > 0)
            {
                Collider target = null;
                foreach (Collider potentialTarget in feastablesInRange)
                {
                    // Debug.Log(name + " Checking: " + potentialTarget);
                    if (potentialTarget.gameObject == gameObject)
                    {
                        // Debug.Log(name + " Target Was Self; Continue");
                        continue;
                    }
                    else
                    {
                        target = potentialTarget;
                        // Debug.Log(name + " Selected Target: " + target + "; Breaking");
                        break;
                    }
                }

                // Debug.Log(name + ": Target Set to: " + target);

                if (target != null)
                {
                    if (target.TryGetComponent(out FeastableEntity feastable))
                    {
                        if (!feastable.Targeted)
                        {
                            feasting = feastable;
                            // Debug.Log(name + " Permitted to Feast On " + target.name + ", Now Feast Locked");
                            feastLockedOn = true;
                            feastable.Targeted = true;

                            StopCoroutine(currentState);

                            currentState = StartCoroutine(DoFeast(feastable));

                            yield return currentState;
                        }
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator DoFeast(FeastableEntity target)
    {
        yield return StartCoroutine(Movement.GoToOverridenTarget(target.transform, transform.localScale.x / 2, false, true, false, delegate
        {
            // Debug.Log(name + " Reached Override Target");
            Movement.SetMove(false);
        }));

        yield return StartCoroutine(Feast(target));

        Movement.SetMove(true);

        currentState = StartCoroutine(AttackCycle());
    }

    private IEnumerator Feast(FeastableEntity toFeastOn)
    {
        isFeasting = true;

        for (float t = 0; t < feastTime; t += Time.deltaTime)
        {
            if (toFeastOn == null)
                yield break;
            yield return null;
        }
        // Debug.Log(name + " Feasted On " + toFeastOn.name);

        KillableEntity myKillableEntity = GetComponent<KillableEntity>();
        FeastableEntity feastingOnEntity = toFeastOn.GetComponent<FeastableEntity>();
        feastingOnEntity.FeastOn();

        if (toFeastOn.TryGetComponent(out FeastingEnemy enemy))
        {
            for (int i = 0; i <= enemy.FeastLevel; i++)
            {
                OnFeast(myKillableEntity);
            }
        }
        else
        {
            OnFeast(myKillableEntity);
        }

        feasting = null;
        feastLockedOn = false;
        feastLevel++;
        currentState = StartCoroutine(AttackCycle());
        isFeasting = false;
    }

    public void RemoveTargeted()
    {
        if (feasting)
            feasting.Targeted = false;
    }

    private void OnFeast(KillableEntity myKillableEntity)
    {
        targetScale += Vector3.one * onFeastSizeChange;
        if (onFeastHPChange != 0)
        {
            myKillableEntity.MaxHealth *= onFeastHPChange;
        }
        myKillableEntity.ResetHealth();
    }
}
