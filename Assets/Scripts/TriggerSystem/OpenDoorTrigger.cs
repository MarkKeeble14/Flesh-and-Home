using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Cinemachine;

public class OpenDoorTrigger : TextPromptKeyTrigger
{
    [SerializeField] private Transform door;
    [SerializeField] private bool isSlidingDoor = true;
    [SerializeField] private SlideDirection slideDirection;
    [SerializeField] private float slideSpeed = 1.0f;
    [SerializeField] private float nudgeAmount = .125f;

    [SerializeField] private CinemachineImpulseSource shakeSource;
    [SerializeField] private float shakeStrength;

    private bool isOpen;
    private Vector3 closePosition;
    private Vector3 openPosition;
    private Coroutine animationCoroutine;

    [SerializeField] private bool allowPlayerControl = true;

    protected override bool AllowShowText => allowPlayerControl;

    [SerializeField] private Renderer triggerRenderer;

    private new void Awake()
    {
        closePosition = door.localPosition;
        switch (slideDirection)
        {
            case SlideDirection.DOWN:
                openPosition = closePosition - door.localScale.y * Vector3.up - (door.localScale.y * nudgeAmount * Vector3.up);
                break;
            case SlideDirection.RIGHT:
                openPosition = closePosition + door.localScale.x * Vector3.right + (door.localScale.x * nudgeAmount * Vector3.right);
                break;
            case SlideDirection.LEFT:
                openPosition = closePosition - door.localScale.x * Vector3.right - (door.localScale.x * nudgeAmount * Vector3.right);
                break;
            case SlideDirection.UP:
                openPosition = closePosition + door.localScale.y * Vector3.up + (door.localScale.y * nudgeAmount * Vector3.up);
                break;
        }

        onActivate += delegate
        {
            if (!allowPlayerControl) return;
            if (!isOpen)
            {
                Open(null);
            }
            else
            {
                /*
                if (isSlidingDoor)
                {
                    Close(null);
                }
                */
            }
            // helperText.Show(this, GetHelperTextString());
            // InputManager._Instance.PlayerInputActions.Player.Interact.started += CallActivate;
        };

        base.Awake();
    }

    public void LockClosed()
    {
        Close(null);
        allowPlayerControl = false;
        showText = false;
    }

    public void LockOpened()
    {
        Open(null);
        allowPlayerControl = false;
        showText = false;
    }

    public void Open(Action onOpened)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        if (isSlidingDoor)
        {
            animationCoroutine = StartCoroutine(SlideOpen(onOpened));
            allowPlayerControl = false;
            showText = false;
        }
    }

    public void Close(Action onClosed)
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isSlidingDoor)
            {
                animationCoroutine = StartCoroutine(SlideClose(onClosed));
            }
        }
    }

    private IEnumerator SlideOpen(Action onOpened)
    {
        Vector3 beginOpenPos = door.localPosition;
        float time = 0;
        isOpen = true;

        while (door.localPosition != openPosition)
        {
            door.localPosition = Vector3.Lerp(beginOpenPos, openPosition, time);

            shakeSource.GenerateImpulse(shakeStrength);

            time += Time.deltaTime * slideSpeed;

            yield return null;
        }

        onOpened?.Invoke();
    }

    private IEnumerator SlideClose(Action onClosed)
    {
        Vector3 beginClosePos = door.localPosition;
        float time = 0;
        isOpen = false;

        while (door.localPosition != closePosition)
        {
            door.localPosition = Vector3.Lerp(beginClosePos, closePosition, time);

            shakeSource.GenerateImpulse(shakeStrength);

            time += Time.deltaTime * slideSpeed;

            yield return null;
        }

        onClosed?.Invoke();
    }

    private void Update()
    {
        if (!allowPlayerControl)
        {
            Prefix = "Door Locked";
            Suffix = "";
        }
        else
        {
            Prefix = "'E' to";
            Suffix = (isOpen ? " Close" : " Open") + " Door";
        }
        triggerRenderer.enabled = allowPlayerControl;
    }
}
