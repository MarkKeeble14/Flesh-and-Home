using UnityEngine;

public class ShockwaveRingColliderCheck : MonoBehaviour
{
    [SerializeField] private ShockwaveRing shockwaveRing;
    public LayerMask canHit;
    public Vector3 relativeScale = new Vector3(.9f, 100f, .9f);
    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, canHit)) return;

        shockwaveRing.AddColliderToHasCollidedWith(other);
    }

    private void Update()
    {
        transform.localScale = relativeScale;
    }
}
