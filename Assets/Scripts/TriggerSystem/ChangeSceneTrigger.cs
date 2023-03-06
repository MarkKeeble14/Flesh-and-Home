using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeSceneTrigger : TextPromptKeyTrigger
{
    [SerializeField] private string sceneName;

    private new void Awake()
    {
        onActivate += () => SceneManager.LoadScene(sceneName);
        base.Awake();
    }
}
