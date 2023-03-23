using UnityEngine;
using UnityEngine.InputSystem;
using System;

public abstract class EventTrigger : MonoBehaviour
{
    protected Action onActivate;
    private bool active = true;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    private new Renderer renderer;
    [SerializeField] private bool useRenderer = true;

    protected void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void AddOnActivate(Action action)
    {
        onActivate += action;
    }

    protected virtual void CallActivate(InputAction.CallbackContext ctx)
    {
        Activate();
    }

    protected void Activate()
    {
        onActivate?.Invoke();
    }

    protected virtual void OnEnter(Collider other)
    {
        //
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
        renderer.enabled = active && useRenderer;
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
