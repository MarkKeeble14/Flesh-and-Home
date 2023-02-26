using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class OpenDoorTrigger : TextPromptKeyTrigger
{
    [SerializeField] private bool isSlidingDoor = true;
    [SerializeField] private SlideDirection slideDirection;
    private bool isOpen;

    private Vector3 startPosition;

    [SerializeField] private float slideAmount = 2f;
    [SerializeField] private float slideSpeed = 1.0f;
    [SerializeField] private float slideDuration = 3f;
    private Coroutine animationCoroutine;
    [SerializeField] private Transform door;

    protected override string Suffix => (isOpen ? " Close" : " Open") + " Door";
    private bool allowPlayerControl = true;

    private void Awake()
    {
        startPosition = door.position;
    }

    public void LockClosed()
    {
        Close(() => Destroy(gameObject));
        allowPlayerControl = false;
        showText = false;
    }

    protected override void Activate()
    {
        if (!allowPlayerControl) return;
        if (!isOpen)
        {
            Open(null);
        }
        else
        {
            if (isSlidingDoor)
            {
                Close(null);
            }
        }
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
        Vector3 endPosition = GetEndPosition();
        float time = 0;
        isOpen = true;

        while (time < slideDuration)
        {
            door.position = Vector3.Lerp(startPosition, endPosition, time);

            time += Time.deltaTime * slideSpeed;

            yield return null;
        }

        onOpened?.Invoke();
    }

    private IEnumerator SlideClose(Action onClosed)
    {
        Vector3 endPosition = startPosition;
        Vector3 beginClosePos = door.position;

        float time = 0;

        isOpen = false;

        while (time < slideDuration)
        {
            door.position = Vector3.Lerp(beginClosePos, endPosition, time);

            time += Time.deltaTime * slideSpeed;

            yield return null;
        }

        onClosed?.Invoke();
    }

    private Vector3 GetEndPosition()
    {
        switch (slideDirection)
        {
            case SlideDirection.DOWN:
                return startPosition + slideAmount * -transform.up;
            case SlideDirection.RIGHT:
                return startPosition + slideAmount * transform.right;
            case SlideDirection.LEFT:
                return startPosition + slideAmount * -transform.right;
            case SlideDirection.UP:
                return startPosition + slideAmount * transform.up;
            default:
                throw new Exception("Unhanlded Switch Case in Open Door Trigger");
        }
    }
}
