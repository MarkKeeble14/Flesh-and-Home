using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RadialMenuButton : MonoBehaviour
{
    public string Name;
    public Color NormalColor = Color.white;
    public Color HighlightedColor = Color.grey;
    public Color PressedColor = Color.grey;
    public UnityEvent onPressEvent;
    public Vector2 AngleBounds;

    [Header("References")]
    [SerializeField] private Image image;
    public Image Image => image;
    private RectTransform rect;
    public new RectTransform transform => rect;

    [SerializeField] private Image spriteImage;
    [SerializeField] private Sprite sprite;
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform detailsTransform;
    [SerializeField] private float dampXShift = 1.5f;
    [SerializeField] private float dampYShift = 3f;

    private void Awake()
    {
        // Get references
        rect = GetComponent<RectTransform>();
        detailsTransform = transform.GetChild(0).GetComponent<RectTransform>();

        // Set
        spriteImage.sprite = sprite;
        text.text = Name;

        // Set default color
        image.color = NormalColor;
    }

    private void Update()
    {
        // Move details position and rotation
        detailsTransform.transform.eulerAngles = new Vector3(0, 0, 360f - rect.eulerAngles.z);
        detailsTransform.anchoredPosition = new Vector2(-rect.rect.width / dampXShift, -rect.rect.height / dampYShift);
    }
}
