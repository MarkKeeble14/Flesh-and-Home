using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    private IRoomContent[] enableOnEnter;
    private List<RoomEnemy> roomEnemies = new List<RoomEnemy>();

    [SerializeField] private CinemachineVirtualCamera mainCam; //normal camera
    [SerializeField] private CinemachineVirtualCamera virtualCam; //pan camera

    [SerializeField] private Transform roomAbove;

    private bool normalCamera = true;
    public float rotationTime = 2.0f;

    private Transform originalLookAt;
    private bool isRotating = false;


    public void OnEnter()
    {
        // Debug.Log(name + " Entered For the First Time");

        if (name == "HubRoom")
        {
            if (normalCamera)
            {
                mainCam.Priority = 9;
                virtualCam.Priority = 10;
                StartCoroutine(RotateCameraUp());
            }
            else
            {
                mainCam.Priority = 10;
                virtualCam.Priority = 9;
            }

            normalCamera = !normalCamera;
        }

        // Get all room content
        enableOnEnter = GetComponentsInChildren<IRoomContent>();

        // Activate all room content
        foreach (IRoomContent roomContent in enableOnEnter)
        {
            roomContent.Activate();
        }

        // 
        roomEnemies.AddRange(GetComponentsInChildren<RoomEnemy>());
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.GetComponent<KillableEntity>().AddAdditionalOnEndAction(() => roomEnemies.Remove(roomEnemy));
            roomEnemy.PlayerIsInRoom = true;
        }
    }
    IEnumerator RotateCameraUp()
    {
        isRotating = true;

        Quaternion originalRotation = virtualCam.transform.rotation;
        Vector3 lookUp = new Vector3(0, 0, 1000);
        Vector3 targetPosition = roomAbove.position + lookUp;

        // Look at the target position
        virtualCam.LookAt = roomAbove;

        // Rotate towards the target position
        float elapsedTime = 0.0f;
        while (elapsedTime < rotationTime)
        {
            //Quaternion.LookRotation(targetPosition - virtualCam.transform.position)
            virtualCam.transform.rotation = Quaternion.Slerp(originalRotation, Quaternion.LookRotation(targetPosition - virtualCam.transform.position), elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the camera's LookAt
        virtualCam.LookAt = originalLookAt;



        isRotating = false;

        mainCam.Priority = 10;
        virtualCam.Priority = 9;
    }

    public void OnReenter()
    {
        // 
        // Debug.Log(name + " Re-entered");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerEnterRoom();
        }
    }

    public void OnExit()
    {
        // 
        //  Debug.Log(name + " Exited");
        foreach (RoomEnemy roomEnemy in roomEnemies)
        {
            roomEnemy.OnPlayerLeaveRoom();
        }
    }
}
