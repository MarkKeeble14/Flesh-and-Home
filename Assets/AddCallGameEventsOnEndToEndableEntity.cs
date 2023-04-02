using UnityEngine;

[RequireComponent(typeof(EndableEntity))]
public class AddCallGameEventsOnEndToEndableEntity : MonoBehaviour
{
    protected GameEvent[] gameEvents;

    private void Activate()
    {
        // Debug.Log("Activate");
        foreach (GameEvent gameEvent in gameEvents)
        {
            gameEvent.Call();
        }
    }

    private void Awake()
    {
        gameEvents = GetComponents<GameEvent>();
        GetComponent<EndableEntity>().AddAdditionalOnEndAction(delegate
        {
            Activate();
        });
    }
}
