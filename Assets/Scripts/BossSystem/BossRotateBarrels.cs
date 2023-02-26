using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRotateBarrels : MonoBehaviour
{
    [SerializeField] private Vector3 axis;
    [SerializeField] private float changeRate = 1f;
    private float currentSpeed;
    private float targetSpeed;

    public void SetRotateSpeed(float speed)
    {
        targetSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * changeRate);
        transform.Rotate(axis, Time.deltaTime * currentSpeed, Space.Self);
    }
}
