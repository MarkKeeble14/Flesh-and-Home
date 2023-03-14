using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager _Instance { get; private set; }

    private List<PauseCondition> numPauseCommands = new List<PauseCondition>();

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
        Resume(PauseCondition.AWAKE);
    }

    public void Pause(PauseCondition condition)
    {
        if (numPauseCommands.Contains(condition)) return;
        numPauseCommands.Add(condition);
        UpdatePauseState();
    }

    public void Resume(PauseCondition condition)
    {
        if (numPauseCommands.Contains(condition))
        {
            numPauseCommands.Remove(condition);
            UpdatePauseState();
        }
    }

    private void UpdatePauseState()
    {
        if (numPauseCommands.Contains(PauseCondition.ESCAPE))
        {
            Time.timeScale = 0;

            UIManager._Instance.OpenPauseMenu();
            Cursor.visible = true;

            InputManager._Instance.DisableInput();

            IsPaused = true;
        }
        else if (numPauseCommands.Count > 0)
        {
            Time.timeScale = 0;
            UIManager._Instance.OpenPauseMenu();

            Cursor.visible = true;

            InputManager._Instance.DisableInput();

            IsPaused = true;
        }
        else
        {
            Time.timeScale = 1;

            UIManager._Instance.CloseCanvas(CanvasFunction.PAUSE);
            UIManager._Instance.CloseCanvas(CanvasFunction.SETTINGS);

            Cursor.visible = false;

            InputManager._Instance.EnableInput();

            IsPaused = false;
        }
    }
}

public enum PauseCondition
{
    ESCAPE,
    AWAKE
}