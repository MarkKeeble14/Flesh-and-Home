using System.Collections;

public class RoomEnemyWanderState : EnemyState
{
    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        enemy.Movement.SetMove(true);
    }

    public override IEnumerator StateBehaviour(RoomEnemyStateController enemy)
    {
        while (enemy.PlayerIsInRoom)
        {
            yield return null;
        }
        enemy.SwitchState(EnemyStateType.ATTACK);
    }
}
