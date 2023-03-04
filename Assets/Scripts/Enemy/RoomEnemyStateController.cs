using UnityEngine;

public class RoomEnemyStateController : RoomEnemy
{
    [Header("Enemy State Controller")]
    [SerializeField]
    protected SerializableDictionary<EnemyState.EnemyStateType, EnemyState> enemyStates
        = new SerializableDictionary<EnemyState.EnemyStateType, EnemyState>();
    [SerializeField] private EnemyState.EnemyStateType defaultState = EnemyState.EnemyStateType.DEAD;
    [SerializeField] private EnemyState.EnemyStateType aggroState;
    [SerializeField] private EnemyState.EnemyStateType unAggroState;

    private EnemyState currentState;

    [Header("References")]
    [SerializeField] private EnemyMovement movement;
    public EnemyMovement Movement => movement;
    public bool HasMovement => movement != null;

    private bool isDisabled;

    private new void Awake()
    {
        base.Awake();
    }

    protected void Update()
    {
        if (isDisabled) return;

        if (currentState != null)
            currentState.UpdateState(this);
    }

    public override void Activate()
    {
        if (isDisabled) return;

        // Debug.Log(name + ": Enemy Activated");

        base.Activate();

        SwitchState(aggroState);
    }

    public override void Deactivate()
    {
        if (isDisabled) return;

        base.Deactivate();

        if (HasBeenActivated)
            SwitchState(unAggroState);
        else
            SwitchState(defaultState);
    }

    private void SwitchState(EnemyState state)
    {
        if (isDisabled) return;

        if (currentState != null)
        {
            currentState.ExitState(this);
            Debug.Log("Exiting: " + currentState);
        }
        currentState = state;
        currentState.EnterState(this);
        Debug.Log("Entering: " + currentState);
    }

    public void SwitchState(EnemyState.EnemyStateType enemyStateType)
    {
        SwitchState(enemyStates[enemyStateType]);
    }

    public virtual void Disable()
    {
        SwitchState(EnemyState.EnemyStateType.DEAD);
        isDisabled = true;
    }
}
