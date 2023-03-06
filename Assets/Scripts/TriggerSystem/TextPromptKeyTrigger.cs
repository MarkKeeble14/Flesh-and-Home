﻿using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class TextPromptKeyTrigger : EventTrigger
{
    private string prefix = "'E' to ";
    [SerializeField] protected string activePrompt;
    protected virtual string Suffix { get => ""; }
    protected TriggerHelperText helperText;
    protected bool showText = true;

    protected virtual bool AllowShowText
    {
        get
        {
            return true;
        }
    }

    [SerializeField] private bool destroyOnActivate = true;

    protected void Awake()
    {
        if (destroyOnActivate)
            onActivate += () => Destroy(gameObject);
    }

    private void Start()
    {
        // Find Helper Text
        helperText = FindObjectOfType<TriggerHelperText>();
    }

    protected override void OnEnter(Collider other)
    {
        if (showText && AllowShowText)
        {
            helperText.Show(this, GetHelperTextString());
        }

        // Add Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;
    }

    protected override void OnExit(Collider other)
    {
        helperText.Hide(this);

        // Remove Activation Event
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
    }

    protected string GetHelperTextString()
    {
        return prefix + activePrompt + Suffix;
    }

    protected override void CallActivate(InputAction.CallbackContext ctx)
    {
        helperText.Hide(this);
        InputManager._Instance.PlayerInputActions.Player.Interact.started -= CallActivate;
        base.CallActivate(ctx);
    }
}