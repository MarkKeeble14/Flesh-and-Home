using UnityEngine;

public class DestroyObjectGameEvent : GameEvent
{
    [SerializeField] private GameObject[] toDestroy;

    protected override void Activate()
    {
        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }
}
