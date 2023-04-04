using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FpsDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _fpsText;

    private void Start()
    {
        _fpsText = GetComponent<TMP_Text>();
        StartCoroutine(FramePerSecond());
    }

    // Update is called once per frame
    private IEnumerator FramePerSecond()
    {
        while (true)
        {
            int fps = (int) (1.0f / Time.deltaTime);
            DisplayFPS(fps);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DisplayFPS(float fps)
    {
        _fpsText.text = $"{fps} FPS";
    }
}
