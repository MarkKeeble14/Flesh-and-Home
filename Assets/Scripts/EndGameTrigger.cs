using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : EventTrigger
{
    [SerializeField] private string loadSceneName;
    private new void Awake()
    {
        onActivate += delegate
        {
            TransitionManager._Instance.FadeOut(delegate
            {
                SceneManager.LoadScene(loadSceneName);
            }, 1.5f);
        };
        base.Awake();
    }

    protected override void OnEnter(Collider other)
    {
        onActivate?.Invoke();
        base.OnEnter(other);
    }
}
