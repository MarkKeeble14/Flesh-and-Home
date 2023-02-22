using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class OpenDoorTrigger : DestroyTriggerOnActivate
{
    [Header("Sliding Door Config")] [SerializeField]
    private bool isSlidingDoor = true;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Vector3 slideDirection = Vector3.back;
    [SerializeField] public Vector3 startPosition;
    [SerializeField] private float slideAmount = 1.9f;
    [SerializeField] private Coroutine animationCoroutine;
    [SerializeField] private float slideSpeed = 1.0f;
    private void Awake()
    {
        startPosition = transform.position;
    }
    protected override void Activate()
    {

        if (!isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isSlidingDoor)
            {
                animationCoroutine = StartCoroutine(doSlidingOpen());
            }
        }
        // else
        // {
        //     if (isSlidingDoor)
        //     {
        //         close();
        //     }
        // }
    }

    
    private IEnumerator doSlidingOpen()
    
    {
        Vector3 endPosition = startPosition + slideAmount * slideDirection;
        Vector3 startPositon = transform.position;
        float time = 0;
        isOpen = true;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPositon, endPosition, time);
            yield return null;
            time += Time.deltaTime * slideSpeed;
        }
    }

    public void close()
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isSlidingDoor)
            {
                animationCoroutine = StartCoroutine(doSlidingClose());
            }
        }
    }
    
    private IEnumerator doSlidingClose()
    {
        Vector3 endPosition = startPosition;
        Vector3 StartPosition = transform.position;

        float time = 0;

        isOpen = false;

        while (time < 0)
        {
            transform.position = Vector3.Lerp(StartPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * slideSpeed;
        }
    }
    
}
