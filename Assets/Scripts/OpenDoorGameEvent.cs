using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class OpenDoorGameEvent : GameEvent
{
    [Header("Settings")]
    [SerializeField] private bool isSlidingDoor = true;
    [SerializeField] private bool allowPlayerControl = true;

    [Header("Sliding Settings")]
    [SerializeField] private SlideDirection slideDirection;
    [SerializeField] private float slideSpeed = 1.0f;
    [SerializeField] private float nudgeAmount = .125f;

    [Header("Shaking")]
    [SerializeField] private CinemachineImpulseSource shakeSource;
    [SerializeField] private float shakeStrength;

    private bool isOpen;
    private Vector3 closePosition;
    private Vector3 openPosition;
    private Coroutine animationCoroutine;

    [Header("References")]
    [SerializeField] private Transform door;
    private TriggerHelperText helperText;

    private void Awake()
    {
        helperText = FindObjectOfType<TriggerHelperText>();

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
    }

    protected override void Activate()
    {
        if (!allowPlayerControl) return;
        if (!isOpen)
        {
            LockOpened();
        }
    }

    public void LockClosed()
    {
        Close(null);
        allowPlayerControl = false;
    }

    public void LockOpened()
    {
        Open(null);
        Active = false;
        allowPlayerControl = false;
        helperText.Hide(this);
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
        Active = allowPlayerControl;
        if (allowPlayerControl)
        {
            if (isOpen)
            {
                EventString = "Close Door";
            }
            else
            {
                EventString = "Open Door";
            }
        }
        else
        {
            EventString = "Door Locked";
        }
    }
}
