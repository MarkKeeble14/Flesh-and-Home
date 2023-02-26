using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    // Make a singleton for easy access + also there should obviously only ever be one crosshair
    public static CrosshairManager _Instance;
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance);
        }
        _Instance = this;
    }

    [SerializeField] private SerializableDictionary<CrosshairType, CrosshairController> crosshairs = new SerializableDictionary<CrosshairType, CrosshairController>();

    public void Spread(CrosshairType type, float force)
    {
        crosshairs[type].Spread(force);
    }

    public void SetCrosshairActive(CrosshairType type)
    {
        crosshairs[type].gameObject.SetActive(true);
    }
}
