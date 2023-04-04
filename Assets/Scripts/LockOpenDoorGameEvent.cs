using System.Collections;
using UnityEngine;

public class LockOpenDoorGameEvent : GameEvent
{
    [SerializeField] private OpenDoorGameEvent door;
    [Tooltip("Open = true - Close = false")]
    [SerializeField] private bool openOrClosed;
    [SerializeField] private float delay;
    [SerializeField] private CoroutineCaller coroutineCaller;

    protected override void Activate()
    {
        coroutineCaller = Instantiate(coroutineCaller, transform.position, Quaternion.identity);
        coroutineCaller.StartCoroutine(OnActivate());
    }

    private IEnumerator OnActivate()
    {
        yield return new WaitForSeconds(delay);
        if (openOrClosed)
        {
            door.LockOpened();
        }
        else
        {
            door.LockClosed();
        }
    }
}
