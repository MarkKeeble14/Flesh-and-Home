using UnityEngine;

public class AggroEnemyGameEvent : GameEvent
{
    [SerializeField] private RoomEnemyStateController[] toAggro;

    protected override void Activate()
    {
        foreach (RoomEnemyStateController enemy in toAggro)
        {
            enemy.GetComponent<NavMeshEnemy>().EnableNavMeshAgent();
            enemy.OnPlayerEnterRoom();
            enemy.gameObject.layer = 17;
        }
    }
}
