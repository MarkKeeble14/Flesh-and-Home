using UnityEngine;

public abstract class RoomEnemy : AttackingEnemy, IRoomContent
{
    [Header("Room Enemy Settings")]
    [SerializeField] private bool activateOnStart;

    [SerializeField] private RoomEnemySettings roomEnemySettings;
    private KillableEntity killableEntity;

    private new Renderer renderer;
    private bool activated;

    protected void Awake()
    {
        renderer = GetComponent<Renderer>();
        killableEntity = GetComponent<KillableEntity>();
        killableEntity.AcceptDamage = false;
    }

    protected void Start()
    {
        if (activateOnStart)
        {
            Activate();
        }
        else if (!activated)
        {
            Deactivate();
        }
    }

    public virtual void Activate()
    {
        // Debug.Log(name + ": Room Enemy Activated");

        activated = true;

        roomEnemySettings.SetActiveColors(renderer);

        killableEntity.AcceptDamage = true;
    }

    public virtual void Deactivate()
    {
        activated = false;

        // Debug.Log(name + ": Room Enemy Activated");
        roomEnemySettings.SetInactiveColors(renderer);

        killableEntity.AcceptDamage = false;
    }
}
