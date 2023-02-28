using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeSceneTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private string sceneName;

    protected override void Activate()
    {
        SceneManager.LoadScene(sceneName);
    }
}
