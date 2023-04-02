using UnityEngine;

public class EnablePlayercontrolOnOpenDoorGameEvent : GameEvent
{
    [SerializeField] private OpenDoorGameEvent door;
    protected override void Activate()
    {
        door.EnablePlayerControl();
    }
}
