using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToSceneGameEvent : GameEvent
{
    [SerializeField] private string loadSceneName;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private MonoBehaviour coroutineCaller;

    protected override void Activate()
    {
        if (TryGetComponent(out DialogueGameEvent dialogue))
        {
            coroutineCaller = Instantiate(coroutineCaller, transform.position, Quaternion.identity);
            coroutineCaller.StartCoroutine(DialogueFirst(dialogue));
        }
        else
        {
            TransitionManager._Instance.FadeOut(delegate
            {
                SceneManager.LoadScene(loadSceneName);
            }, waitTime);
        }
    }

    private IEnumerator DialogueFirst(DialogueGameEvent dialogue)
    {
        dialogue.Call();

        yield return new WaitUntil(() => !DialogueManager._Instance.Idle);

        yield return new WaitUntil(() => DialogueManager._Instance.Idle);

        TransitionManager._Instance.FadeOut(delegate
        {
            SceneManager.LoadScene(loadSceneName);
        }, waitTime);
    }
}
