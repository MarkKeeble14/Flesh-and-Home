using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class EnterRuinsTrigger : MonoBehaviour
{
    [SerializeField] private string ruinsSceneName = "RuinsScene";
    [SerializeField] private string canLoadString = "Press E to Enter Ruins";
    [SerializeField] private TextMeshProUGUI helperText;

    private bool playerInTrigger;
    private bool loading;
    private bool canLoad
    {
        get { return playerInTrigger && !loading; }
    }

    private void Update()
    {
        if (canLoad)
        {
            helperText.gameObject.SetActive(true);
            helperText.text = canLoadString;
        }
        else
        {
            helperText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        playerInTrigger = false;
    }

    private void Start()
    {
        InputManager._Instance.PlayerInputActions.Player.Interact.started += TryLoadRuins;
    }

    private void TryLoadRuins(InputAction.CallbackContext ctx)
    {
        if (!canLoad) return;
        loading = true;
        SceneManager.LoadScene(ruinsSceneName);
    }
}
