using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private new Light light;
    [SerializeField] private PlayerLaserExplosiveParticleSystem explosionParticleSystem;
    private TestRaygun raygun;

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, raygun.CanHit)) return;
        Explode();
    }

    private IEnumerator Travel(Vector3 target)
    {
        float step = speed * Time.deltaTime;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void setTarget(Vector3 target, TestRaygun raygun)
    {
        this.raygun = raygun;
        light.color = raygun.CurrentColor;
        StartCoroutine(Travel(target));
    }

    void Explode()
    {
        PlayerLaserExplosiveParticleSystem explosion = Instantiate(explosionParticleSystem, transform.position, Quaternion.identity);
        explosion.SetColors(raygun.CurrentColor, raygun.CurrentColor, raygun.CurrentColor);
        Destroy(gameObject);
    }
}