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

    public void OverrideToAggroState()
    {
        defaultState = aggroState;
        unAggroState = aggroState;
        SwitchState(aggroState);
    }

    private new void Awake()
    {
        base.Awake();

        if (TryGetComponent(out KillableEntity killableEntity))
        {
            killableEntity.AddAdditionalOnEndAction(() => OnDeath());
        }

        SwitchState(defaultState);
    }

    protected void Update()
    {
        if (isDisabled) return;

        if (currentState != null)
            currentState.UpdateState(this);
    }

    public override void OnPlayerEnterRoom()
    {
        base.OnPlayerEnterRoom();
        if (isDisabled) return;
        SwitchState(aggroState);
    }

    public override void OnPlayerLeaveRoom()
    {
        base.OnPlayerLeaveRoom();
        if (isDisabled) return;
        SwitchState(unAggroState);
    }

    private void SwitchState(EnemyState state)
    {
        if (isDisabled) return;

        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = state;
        currentState.EnterState(this);
    }

    public void SwitchState(EnemyState.EnemyStateType enemyStateType)
    {
        SwitchState(enemyStates[enemyStateType]);
    }

    protected virtual void OnDeath()
    {
        SwitchState(EnemyState.EnemyStateType.DEAD);
        isDisabled = true;
    }
}
