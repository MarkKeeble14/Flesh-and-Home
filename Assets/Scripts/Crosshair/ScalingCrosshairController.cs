using UnityEngine;

public class ScalingCrosshairController : CrosshairController
{
    private RectTransform rect;
    [SerializeField] private Vector3 restingScale = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 maxScale = new Vector3(1.25f, 1.25f, 1.25f);

    private void Awake()
    {
        // Get reference to rect transform
        rect = GetComponent<RectTransform>();
    }
    public override void Spread(float force)
    {
        rect.localScale = Vector3.Lerp(rect.localScale, maxScale, force * Time.deltaTime * spreadSpeed);
    }

    public override void Release()
    {
        rect.localScale = Vector3.Lerp(rect.localScale, restingScale, Time.deltaTime * releaseSpeed);
    }
}
