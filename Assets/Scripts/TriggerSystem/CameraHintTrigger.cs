using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHintTrigger : EventTrigger
{
    [SerializeField] private List<UnlockType> requiredUnlocks = new List<UnlockType>();
    [SerializeField] private GameObject enableCamera;
    [SerializeField] private float duration;
    [SerializeField] private OpenDoorTrigger openDoorOnActivate;

    private KillableEntity player;

    protected override void OnStay(Collider other)
    {
        if (player == null)
            player = GameManager._Instance.PlayerTransform.GetComponent<KillableEntity>();

        if (!ProgressionManager._Instance.HasAllUnlocks(requiredUnlocks)) return;

        Active = false;

        StartCoroutine(OnActivate());
    }

    private IEnumerator OnActivate()
    {
        InputManager._Instance.DisableInput();
        player.AcceptDamage = false;

        ICinemachineCamera activeCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
        activeCamera.VirtualCameraGameObject.SetActive(false);
        enableCamera.SetActive(true);

        UIManager._Instance.SetBlackBars(true, false);

        openDoorOnActivate.LockOpened();

        yield return new WaitForSeconds(duration);

        enableCamera.SetActive(false);
        activeCamera.VirtualCameraGameObject.SetActive(true);

        UIManager._Instance.SetBlackBars(false, false);

        InputManager._Instance.EnableInput();
        player.AcceptDamage = true;

        Destroy(gameObject);
    }
}
