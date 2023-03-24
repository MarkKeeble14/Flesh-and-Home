using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using System.Collections;

public class CameraHintGameEvent : GameEvent
{
    [SerializeField] private List<UnlockType> requiredUnlocks = new List<UnlockType>();
    [SerializeField] private GameObject enableCamera;
    [SerializeField] private float duration;
    [SerializeField] private OpenDoorGameEvent openDoorOnActivate;
    private KillableEntity player;
    [SerializeField] private MonoBehaviour dummyObj;
    private MonoBehaviour spawnedDummy;

    private void Update()
    {
        Active = ProgressionManager._Instance.HasAllUnlocks(requiredUnlocks);
    }

    private IEnumerator Execute()
    {
        // Disable Player
        InputManager._Instance.DisableInput();
        GameManager._Instance.PlayerHealth.AcceptDamage = false;

        // Enable Cinematic View
        UIManager._Instance.SetBlackBars(true, false);

        // Switch Cameras
        ICinemachineCamera activeCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
        activeCamera.VirtualCameraGameObject.SetActive(false);
        enableCamera.SetActive(true);

        // Open door if defined to
        if (openDoorOnActivate != null)
        {
            openDoorOnActivate.LockOpened();
        }

        // Wait some time
        yield return new WaitForSeconds(duration);

        // Switch back to initial camera
        enableCamera.SetActive(false);
        activeCamera.VirtualCameraGameObject.SetActive(true);

        // Disable Cinematic view
        UIManager._Instance.SetBlackBars(false, false);

        // Re-enable player
        InputManager._Instance.EnableInput();
        GameManager._Instance.PlayerHealth.AcceptDamage = true;

        // Destroy the object with which the coroutine is running on
        Destroy(spawnedDummy.gameObject);
    }

    protected override void Activate()
    {
        spawnedDummy = Instantiate(dummyObj);
        spawnedDummy.StartCoroutine(Execute());
        Active = false;
    }
}
