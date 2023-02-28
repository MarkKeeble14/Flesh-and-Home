using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    private int escapeToggle;
    [SerializeField] private SerializableDictionary<CanvasFunction, GameObject> canvases = new SerializableDictionary<CanvasFunction, GameObject>();

    private void Start()
    {
        InputManager._Instance.PlayerInputActions.Player.Escape.performed += Escape;
    }

    public void OpenSettingsMenu()
    {
        OpenCanvas(CanvasFunction.SETTINGS);
    }

    public void OpenPauseMenu()
    {
        OpenCanvas(CanvasFunction.PAUSE);
    }

    public void OpenCanvas(CanvasFunction canvas)
    {
        foreach (GameObject obj in canvases.Values())
        {
            obj.SetActive(false);
        }
        canvases[canvas].SetActive(true);
    }

    public void CloseCanvas(CanvasFunction canvas)
    {
        canvases[canvas].SetActive(false);
    }


    public void Escape(InputAction.CallbackContext ctx)
    {
        if (escapeToggle == 0)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        PauseMenuManager._Instance.Pause(PauseCondition.ESCAPE);
        escapeToggle++;

    }

    public void ResumeGame()
    {
        PauseMenuManager._Instance.Resume(PauseCondition.ESCAPE);
        escapeToggle--;

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

