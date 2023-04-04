using UnityEngine;

public abstract class EventTriggerer : MonoBehaviour
{
    protected abstract bool Condition { get; }

    protected GameEvent[] gameEvents;

    private void Awake()
    {
        gameEvents = GetComponents<GameEvent>();
    }

    private void Activate()
    {
        // Debug.Log("Activate");
        foreach (GameEvent gameEvent in gameEvents)
        {
            gameEvent.Call();
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Condition)
        {
            Activate();
        }
    }
}
