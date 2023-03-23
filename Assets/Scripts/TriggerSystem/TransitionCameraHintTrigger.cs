using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class TransitionCameraHintTrigger : TextPromptKeyTrigger
{
    [SerializeField] private CinemachineVirtualCamera enableCamera;
    [SerializeField] private float duration;
    private KillableEntity player;

    [SerializeField] private OpenDoorTrigger openDoor;

    [SerializeField] private UnlockType unlock;

    protected override string Suffix => GetUnlockTypeString(unlock);

    private new void Awake()
    {
        onActivate += delegate
        {
            if (!Active) return;
            StartCoroutine(OnActivate());
            Active = false;
            helperText.Hide(this);
        };
        base.Awake();
    }

    private string GetUnlockTypeString(UnlockType unlock)
    {
        switch (unlock)
        {
            case UnlockType.JETPACK:
                return "Jetpack";
            case UnlockType.FLAMETHROWER:
                return "Flamethrower";
            case UnlockType.LASER_CUTTER:
                return "Laser Cutter";
            case UnlockType.PULSE_GRENADE_LAUNCHER:
                return "Pulse Grenade Launcher";
            default:
                throw new System.Exception("Unhandled Switch Case in Progression Trigger");
        }
    }

    private IEnumerator OnActivate()
    {
        InputManager._Instance.DisableInput();

        player = GameManager._Instance.PlayerTransform.GetComponent<KillableEntity>();
        player.AcceptDamage = false;

        bool hasFaded = false;
        TransitionManager._Instance.FadeOut(() => hasFaded = true);
        yield return new WaitUntil(() => hasFaded);
        UIManager._Instance.SetBlackBars(true, true);

        ICinemachineCamera povCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
        povCamera.VirtualCameraGameObject.SetActive(false);
        enableCamera.gameObject.SetActive(true);

        yield return new WaitUntil(() => CinemachineCore.Instance.GetActiveBrain(0).IsBlending);
        yield return new WaitUntil(() => !CinemachineCore.Instance.GetActiveBrain(0).IsBlending);

        hasFaded = false;
        TransitionManager._Instance.FadeIn(delegate
        {
            hasFaded = true;
        });
        yield return new WaitUntil(() => hasFaded);

        if (openDoor)
            openDoor.LockOpened();

        yield return new WaitForSeconds(duration);

        hasFaded = false;
        TransitionManager._Instance.FadeOut(delegate
        {
            hasFaded = true;
        });

        yield return new WaitUntil(() => hasFaded);
        UIManager._Instance.SetBlackBars(false, true);

        enableCamera.gameObject.SetActive(false);
        povCamera.VirtualCameraGameObject.SetActive(true);

        yield return new WaitUntil(() => CinemachineCore.Instance.GetActiveBrain(0).IsBlending);
        yield return new WaitUntil(() => !CinemachineCore.Instance.GetActiveBrain(0).IsBlending);

        hasFaded = false;
        TransitionManager._Instance.FadeIn(() => hasFaded = true);
        yield return new WaitUntil(() => hasFaded);

        switch (unlock)
        {
            case UnlockType.JETPACK:
                ProgressionManager._Instance.UnlockJetpack();
                break;
            case UnlockType.FLAMETHROWER:
                ProgressionManager._Instance.UnlockFlamethrower();
                break;
            case UnlockType.LASER_CUTTER:
                ProgressionManager._Instance.UnlockLaserCutter();
                break;
            case UnlockType.PULSE_GRENADE_LAUNCHER:
                ProgressionManager._Instance.UnlockPulseGrenadeLauncher();
                break;
        }

        InputManager._Instance.EnableInput();
        player.AcceptDamage = true;

        Destroy(gameObject);
    }
}
