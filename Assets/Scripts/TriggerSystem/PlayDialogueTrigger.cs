using UnityEngine;

public class PlayDialogueTrigger : EventTrigger
{
    [SerializeField] private Dialogue onTriggerDialogue;

    private new void Awake()
    {
        onActivate += delegate
        {
            onTriggerDialogue.Play();
        };
        base.Awake();
    }

    protected override void OnEnter(Collider other)
    {
        onActivate?.Invoke();
        base.OnEnter(other);
        Destroy(gameObject);
    }
}
