using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairComponent : MonoBehaviour
{
    private RectTransform rt;
    [Tooltip("Representation of the direction/responsiblity of the component; i.e., <1, 0> would mean it is the right component, <0, 1> would be the top component, etc")]
    [SerializeField] private Vector2 pos;
    [SerializeField] private float restScale = 20f;
    [SerializeField] private float maxScale = 60f;
    private Vector2 maxPos;
    private Vector2 restingPos;

    private void Awake()
    {
        // Get reference to rect transform
        rt = GetComponent<RectTransform>();

        // Set scales
        restingPos = pos * restScale;
        maxPos = pos * maxScale;
    }

    public void Spread(float force, float spreadSpeed)
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, maxPos, Time.deltaTime * spreadSpeed * force);
    }

    public void Release(float releaseSpeed)
    {
        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, restingPos, Time.deltaTime * releaseSpeed);
    }
}
