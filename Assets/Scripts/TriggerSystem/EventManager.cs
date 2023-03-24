using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class EventManager : MonoBehaviour
{
    private bool active = true;

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    private Action onActivate;

    private new Renderer renderer;
    protected GameEvent[] gameEvents;
    [SerializeField] private bool useRenderer = true;
    [SerializeField] protected bool destroyOnActivate = true;
    [SerializeField] private Color triggerColor;
    public virtual bool ActivateOnCollide { get { return true; } }

    protected void Awake()
    {
        // Set references
        renderer = GetComponent<Renderer>();
        renderer.material.color = triggerColor;
        gameEvents = GetComponents<GameEvent>();

        if (destroyOnActivate)
        {
            AddOnActivate(() => Destroy(gameObject));
        }
    }

    public void AddOnActivate(Action action)
    {
        onActivate += action;
    }

    protected virtual void CallActivate(InputAction.CallbackContext ctx)
    {
        Activate();
    }

    private void Activate()
    {
        foreach (GameEvent gameEvent in gameEvents)
        {
            gameEvent.Call();
        }
        onActivate?.Invoke();

        if (destroyOnActivate)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnEnter(Collider other)
    {
        //
        if (ActivateOnCollide)
        {
            Activate();
        }
    }
    protected virtual void OnExit(Collider other)
    {
        //
    }

    protected virtual void OnStay(Collider other)
    {
        //
    }

    protected void Update()
    {
        // if one of the game events in inactive, do not allow the trigger to be called
        foreach (GameEvent gameEvent in gameEvents)
        {
            if (!gameEvent.Active)
            {
                active = false;
                renderer.enabled = false;
                return;
            }
        }

        // none of the game events are inactive
        active = true;
        renderer.enabled = useRenderer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        OnEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!active) return;
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        OnExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active) return;
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        OnStay(other);
    }
}
