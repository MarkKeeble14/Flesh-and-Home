using System.Collections;
using UnityEngine;

public abstract class RoomEnemy : AttackingEnemy, IRoomContent
{
    [Header("Room Enemy Settings")]
    [SerializeField] private bool activateOnStart;

    [SerializeField] private RoomEnemySettings roomEnemySettings;
    private KillableEntity killableEntity;

    public RoomController SpawnRoom { get; set; }
    private new Renderer renderer;
    private bool activated;

    public bool PlayerIsInRoom { get; set; }
    public bool HasBeenActivated { get; private set; }

    protected void Awake()
    {
        renderer = GetComponent<Renderer>();
        killableEntity = GetComponent<KillableEntity>();
        killableEntity.AcceptDamage = false;

        SpawnRoom = Physics.OverlapSphere(transform.position, .1f, LayerMask.GetMask("RoomTrigger"))[0].GetComponent<RoomController>();
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
        HasBeenActivated = true;

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

    public void Aggro()
    {
        PlayerIsInRoom = true;
        Activate();
    }

    public void Deaggro()
    {
        PlayerIsInRoom = false;
        Deactivate();
    }
}
