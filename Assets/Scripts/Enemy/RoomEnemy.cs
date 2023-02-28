using UnityEngine;

public abstract class RoomEnemy : AttackingEnemy, IRoomContent
{
    [Header("Room Enemy Settings")]
    [SerializeField] private bool activateOnStart;

    [SerializeField] private RoomEnemySettings roomEnemySettings;
    private KillableEntity killableEntity;

    private new Renderer renderer;

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
        else
        {
            roomEnemySettings.SetInactiveColors(renderer);
        }
    }

    public virtual void Activate()
    {
        roomEnemySettings.SetActiveColors(renderer);
        killableEntity.AcceptDamage = true;
    }
}
