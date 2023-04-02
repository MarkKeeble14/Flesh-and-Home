public class IdleEnemyState : EnemyState
{
    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        enemy.Movement.SetMove(false);
    }
}
