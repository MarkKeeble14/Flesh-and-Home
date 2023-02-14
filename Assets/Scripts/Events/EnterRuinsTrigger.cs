using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterRuinsTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private string ruinsSceneName = "RuinsScene";

    protected override void Activate()
    {
        SceneManager.LoadScene(ruinsSceneName);
    }
}
