using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairMemberController : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector2 restingPos;
    [SerializeField] private Vector2 maxPos;
    [SerializeField] private float spreadSpeed;
    [SerializeField] private float releaseSpeed;

    public void Spread(float force)
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, maxPos, Time.deltaTime * spreadSpeed * force);
    }

    public void Release()
    {
        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, restingPos, Time.deltaTime * releaseSpeed);
    }
}
