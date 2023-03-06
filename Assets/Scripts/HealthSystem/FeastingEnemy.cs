using UnityEngine;

public class FeastingEnemy : MonoBehaviour
{
    [SerializeField] private float scaleSpeed = 2.5f;
    [SerializeField] private int feastLevel = 1;

    [Tooltip("Additive")]
    [SerializeField] private float onFeastSizeChange;
    [Tooltip("Multiplicative")]
    [SerializeField] private float onFeastHPChange;

    private KillableEntity health;
    public KillableEntity Health => health;

    public int FeastLevel => feastLevel;
    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;

        health = GetComponent<KillableEntity>();
    }

    private void Update()
    {
        // Update Scale
        if (transform.localScale != targetScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
    }

    public void FeastOnEntity(FeastableEntity feastableEntity)
    {
        // TODO
        if (feastableEntity.TryGetComponent(out FeastingEnemy feastingEnemy))
        {
            for (int i = 0; i <= feastingEnemy.FeastLevel; i++)
            {
                DoOnFeast();
            }
        }
        else
        {
            // Debug.Log(name + " Feasted On " + currentTarget.name);
            DoOnFeast();
        }
    }

    public void DoOnFeast()
    {
        targetScale += Vector3.one * onFeastSizeChange;
        if (onFeastHPChange != 0)
        {
            health.MaxHealth *= onFeastHPChange;
        }
        health.ResetHealth();
        feastLevel++;
    }
}
