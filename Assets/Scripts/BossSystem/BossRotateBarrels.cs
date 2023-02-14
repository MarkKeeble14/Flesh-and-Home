using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRotateBarrels : MonoBehaviour
{
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, Time.deltaTime * rotateSpeed, Space.Self);
    }
}
