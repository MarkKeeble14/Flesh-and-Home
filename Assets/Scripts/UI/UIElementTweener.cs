using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIElementTweener : MonoBehaviour
{
    [SerializeField] private float scaleToMultiplier = 1.25f;
    [SerializeField] private float inDuration = .5f;
    [SerializeField] private float outDuration = .5f;
    [SerializeField] private Ease inEase = Ease.InOutSine;
    [SerializeField] private Ease outEase = Ease.InOutSine;
    private Vector3 originalScale;
    private Vector3 scaleTo;

    private void Start()
    {
        originalScale = transform.localScale;
        scaleTo = originalScale * scaleToMultiplier;
    }

    public void Scale()
    {
        transform.DOComplete();
        transform.DOScale(scaleTo, inDuration)
            .SetEase(inEase)
            .SetUpdate(true);
    }

    public void Descale()
    {
        transform.DOComplete();
        transform.DOScale(originalScale, outDuration)
            .SetEase(outEase)
            .SetUpdate(true);
    }
}
