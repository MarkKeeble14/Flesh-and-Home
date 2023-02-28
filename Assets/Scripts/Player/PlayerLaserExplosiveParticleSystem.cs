using UnityEngine;

public class PlayerLaserExplosiveParticleSystem : MonoBehaviour
{
    [SerializeField] private Material smoke;
    [SerializeField] private Material explode;
    [SerializeField] private float explosiveEmissionIntensity = 10f;
    [SerializeField] private Material sparks;
    [SerializeField] private float sparksEmissionIntensity = 10f;

    public void SetColors(Color smokeColor, Color explosionColor, Color sparksColor)
    {
        smoke.color = smokeColor;
        explode.color = explosionColor;
        explode.SetColor("_EmissionColor", explosionColor * explosiveEmissionIntensity);
        sparks.color = sparksColor;
        sparks.SetColor("_EmissionColor", sparksColor * sparksEmissionIntensity);
    }
}
