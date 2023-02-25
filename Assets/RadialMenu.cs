using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private List<RadialMenuButton> buttons = new List<RadialMenuButton>();
    [SerializeField] private float offset;
    private int curMenuItem;
    private int oldMenuItem;
    private float currentAngle;

    private void Update()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            RadialMenuButton button = buttons[i];

            // set fill amount of each image
            float fillAmount = 1.0f / buttons.Count;
            button.Image.fillAmount = fillAmount;

            // set rotation of each image
            Vector3 newRot = new Vector3(0, 0, i * (360.0f / buttons.Count));
            button.AngleBounds = new Vector2((i == 0 ? 0 : buttons[i - 1].AngleBounds.y), (i + 1) * (360.0f / buttons.Count));
            button.transform.eulerAngles = newRot;
        }
        GetCurrentMenuItem();
    }

    // Start is called before the first frame update
    void Start()
    {
        curMenuItem = 0;
        oldMenuItem = 0;
    }

    private void OnEnable()
    {
        InputManager._Instance.PlayerInputActions.Player.Shoot.started += ButtonAction;
    }

    private void OnDisable()
    {
        InputManager._Instance.PlayerInputActions.Player.Shoot.started -= ButtonAction;
    }

    public void GetCurrentMenuItem()
    {
        Vector2 mousePosition = InputManager._Instance.PlayerInputActions.Player.MousePosition.ReadValue<Vector2>();
        Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        currentAngle = Mathf.Rad2Deg *
            Mathf.Atan2(centerPoint.y - mousePosition.y, centerPoint.x - mousePosition.x);
        currentAngle += offset;
        if (currentAngle < 0)
            currentAngle += 360;

        for (int i = 0; i < buttons.Count; i++)
        {
            RadialMenuButton button = buttons[i];
            if (currentAngle > button.AngleBounds.x && currentAngle < button.AngleBounds.y)
            {
                curMenuItem = i;
                break;
            }
        }

        if (curMenuItem != oldMenuItem)
        {
            buttons[oldMenuItem].Image.color = buttons[oldMenuItem].NormalColor;
            oldMenuItem = curMenuItem;
            buttons[curMenuItem].Image.color = buttons[curMenuItem].HighlightedColor;
        }
    }

    public void SelectButton()
    {
        buttons[curMenuItem].Image.color = buttons[curMenuItem].PressedColor;
        buttons[curMenuItem].onPressEvent?.Invoke();
    }

    public void ButtonAction(InputAction.CallbackContext ctx)
    {
        SelectButton();
    }
}
