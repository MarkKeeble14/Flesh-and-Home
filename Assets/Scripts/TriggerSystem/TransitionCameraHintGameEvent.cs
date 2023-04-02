using UnityEngine;
using Cinemachine;
using System.Collections;


public class TransitionCameraHintGameEvent : GameEvent
{
    [SerializeField] private CinemachineVirtualCamera enableCamera;
    [SerializeField] private float duration;
    private KillableEntity player;

    [SerializeField] private GameEvent[] postTransitionGameEvents;

    [SerializeField] private UnlockType unlock;
    [SerializeField] private MonoBehaviour dummyObj;
    private MonoBehaviour spawnedDummy;

    private void Update()
    {
        EventString = ProgressionManager.GetUnlockTypeString(unlock);
    }

    protected override void Activate()
    {
        spawnedDummy = Instantiate(dummyObj);
        spawnedDummy.StartCoroutine(Execute());
        Active = false;
    }

    private IEnumerator Execute()
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

        ProgressionManager._Instance.Unlock(unlock, true);

        InputManager._Instance.EnableInput();
        player.AcceptDamage = true;

        foreach (GameEvent gameEvent in postTransitionGameEvents)
        {
            gameEvent.Call();
        }

        Destroy(spawnedDummy.gameObject);
    }
}
