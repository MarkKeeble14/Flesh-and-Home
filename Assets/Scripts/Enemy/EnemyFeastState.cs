using UnityEngine;

public abstract class EnemyFeastState : EnemyState
{
    // Reference
    protected FeastEnemyStateController enemyFeastStateController;

    private void Awake()
    {
        // Get Reference
        enemyFeastStateController = GetComponent<FeastEnemyStateController>();
    }
}
