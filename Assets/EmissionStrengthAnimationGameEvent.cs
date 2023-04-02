using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionStrengthAnimationGameEvent : GameEvent
{
    [SerializeField] private int numFlashes = 1;
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 2f;
    [SerializeField] private float animateDelay = 0f;
    [SerializeField] private float animateToDuration = 1f;
    [SerializeField] private float animateStayDuration = 1f;
    [SerializeField] private float animateFromDuration = 1f;
    [SerializeField] private float rate;
    [SerializeField] private Renderer toAnimate;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private MonoBehaviour spawnedDummy;
    [SerializeField] private OverheatableEntity disable;

    protected override void Activate()
    {
        spawnedDummy = Instantiate(spawnedDummy);
        spawnedDummy.StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        if (disable != null)
        {
            disable.enabled = false;
        }

        Material mat = toAnimate.material;
        Color startColor = mat.GetColor("_EmissionColor");
        mat.SetColor("_EmissionColor", color * min);

        yield return new WaitForSeconds(animateDelay);

        for (int i = 0; i < numFlashes; i++)
        {
            yield return spawnedDummy.StartCoroutine(Flash(mat));
        }
        mat.SetColor("_EmissionColor", startColor);

        if (disable != null)
        {
            disable.enabled = true;
        }

        Destroy(spawnedDummy.gameObject);
    }

    private IEnumerator Flash(Material mat)
    {
        float mult = min;
        float t = 0;

        // To
        while (t < animateToDuration)
        {
            t += Time.deltaTime;

            if (mult <= max)
            {
                mult += Time.deltaTime * rate;
            }
            else
            {
                mult = max;
            }
            mat.SetColor("_EmissionColor", color * mult);

            yield return null;
        }
        t = 0;

        // Stay
        while (t < animateStayDuration)
        {
            t += Time.deltaTime;
            mat.SetColor("_EmissionColor", color * mult);
            yield return null;
        }
        t = 0;

        // From
        while (mult > min)
        {
            mult -= Time.deltaTime * rate;
            if (mult <= min) mult = min;
            mat.SetColor("_EmissionColor", color * mult);

            yield return null;
        }
    }
}
