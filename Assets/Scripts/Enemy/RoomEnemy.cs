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

    public bool PlayerIsInRoom { get; set; }
    public bool HasBeenActivated { get; private set; }
    public bool IsCurrentlyActive { get; private set; }

    protected void Awake()
    {
        renderer = GetComponent<Renderer>();
        killableEntity = GetComponent<KillableEntity>();
        killableEntity.AcceptDamage = false;

        SpawnRoom = Physics.OverlapSphere(transform.position, .1f, LayerMask.GetMask("RoomTrigger"))[0].GetComponent<RoomController>();
    }

    protected void Start()
    {
        if (!HasBeenActivated)
            Deactivate();

        if (activateOnStart)
        {
            Activate();
        }
    }

    public virtual void Activate()
    {
        // Debug.Log(name + ": Room Enemy Activated");

        HasBeenActivated = true;

        IsCurrentlyActive = true;

        roomEnemySettings.SetActiveColors(renderer);

        killableEntity.AcceptDamage = true;
    }

    public virtual void Deactivate()
    {
        // Debug.Log(name + ": Room Enemy Activated");

        IsCurrentlyActive = false;

        roomEnemySettings.SetInactiveColors(renderer);

        killableEntity.AcceptDamage = false;
    }

    public virtual void OnPlayerEnterRoom()
    {
        PlayerIsInRoom = true;
        Activate();
    }

    public virtual void OnPlayerLeaveRoom()
    {
        PlayerIsInRoom = false;
        Deactivate();
    }
}
