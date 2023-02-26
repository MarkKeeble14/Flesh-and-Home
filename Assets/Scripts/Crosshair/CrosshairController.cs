using UnityEngine;

public abstract class CrosshairController : MonoBehaviour
{
    [SerializeField] protected float spreadSpeed = 17.5f;
    [SerializeField] protected float releaseSpeed = 50f;
    public abstract void Spread(float force);
    public abstract void Release();
    protected void Update()
    {
        Release();
    }
}
