using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToSceneGameEvent : GameEvent
{
    [SerializeField] private string loadSceneName;
    [SerializeField] private float waitTime = 1f;

    protected override void Activate()
    {
        TransitionManager._Instance.FadeOut(delegate
        {
            SceneManager.LoadScene(loadSceneName);
        }, waitTime);
    }
}
