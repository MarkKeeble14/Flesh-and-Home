using UnityEngine;
using TMPro;

public class TriggerHelperText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Show(string text)
    {
        this.text.gameObject.SetActive(true);
        this.text.text = text;
    }

    public void Hide()
    {
        text.gameObject.SetActive(false);
    }
}
