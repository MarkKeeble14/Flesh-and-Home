using UnityEngine;

public class DialogueGameEvent : GameEvent
{
    [SerializeField] private Dialogue onTriggerDialogue;

    protected override void Activate()
    {
        onTriggerDialogue.Play();
    }
}
