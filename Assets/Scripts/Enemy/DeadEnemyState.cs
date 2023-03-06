using System.Collections;

public class DeadEnemyState : EnemyState
{

    public override void EnterState(RoomEnemyStateController enemy)
    {
        base.EnterState(enemy);

        // 27 is the int assigned to the "DeadEnemy" Layer
        gameObject.layer = 27;

        enemy.Movement.SetMove(false);
    }
}
