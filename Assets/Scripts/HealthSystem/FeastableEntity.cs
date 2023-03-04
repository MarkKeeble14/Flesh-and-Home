using System;
using UnityEngine;

public class FeastableEntity : KillableEntity
{
    [SerializeField] private int feastableLayer = 19;
    [SerializeField] private float corpseHP;
    private bool isCorpse;
    [SerializeField] private Color corpseColor;

    private FeastEnemyStateController targetedBy;
    public FeastEnemyStateController TargetedBy
    {
        get
        {
            return targetedBy;
        }
        set
        {
            targetedBy = value;
        }
    }

    public Action onBecomeCorpse;
    public Action onCorpseDie;

    [SerializeField] protected new Renderer renderer;

    protected override void OnEnd()
    {
        base.OnEnd();

        if (!isCorpse)
        {
            // Debug.Log(name + ": Now Corpse");

            gameObject.layer = feastableLayer;
            MaxHealth = corpseHP;
            CurrentHealth = corpseHP;

            renderer.material.DisableKeyword("_EMISSION");
            renderer.material.color = corpseColor;

            if (TryGetComponent(out NavMeshEnemy navMeshEnemy))
            {
                navMeshEnemy.DisableNavMeshAgent();
            }

            if (TryGetComponent(out RoomEnemyStateController enemyStateController))
            {
                enemyStateController.Disable();
            }

            acceptDamage = true;
            isCorpse = true;

            // Debug.Log("On Become Corpse Invoked");
            onBecomeCorpse?.Invoke();
        }
        else
        {
            onCorpseDie?.Invoke();
            Destroy(gameObject);
        }
    }

    public void FeastOn()
    {
        onCorpseDie?.Invoke();

        CallOnEndAction();

        // Debug.Log(name + " Was Feasted On");
        Destroy(gameObject);
    }
}
