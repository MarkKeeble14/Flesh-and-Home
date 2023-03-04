using System.Collections;

public class DeadEnemyState : EnemyState
{
    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        enemy.Movement.SetMove(false);
    }

    public override IEnumerator StateBehaviour(RoomEnemyStateController enemy)
    {
        yield break;
    }
}
