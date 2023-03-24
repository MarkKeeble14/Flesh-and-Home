using UnityEngine;

[RequireComponent(typeof(EventManager))]
public abstract class GameEvent : MonoBehaviour
{
    public bool Active { get; protected set; }
    [SerializeField] private bool useText = true;
    public bool UseText => useText;
    [SerializeField] private string label;
    public string Label => label;
    public virtual string EventString { get; protected set; }

    private void Awake()
    {
        Active = true;
    }

    public void Call()
    {
        if (!Active) return;
        Activate();
    }

    protected abstract void Activate();
}
