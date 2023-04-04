using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TMP_Text))]
public class FpsDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _fpsText;

    private InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager._Instance;
        _fpsText = GetComponent<TMP_Text>();
        StartCoroutine(FramePerSecond());
    }

    // Update is called once per frame
    private IEnumerator FramePerSecond()
    {
        while (true)
        {
            float fps = inputManager.PlayerInputActions.Player.Look.ReadValue<Vector2>().x;
            DisplayFPS(fps);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DisplayFPS(float fps)
    {
        _fpsText.text = $"{fps}";
    }
}
