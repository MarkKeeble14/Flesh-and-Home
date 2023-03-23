using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodSpatterSelector : MonoBehaviour
{
    public static BloodSpatterSelector _Instance { get; private set; }

    [SerializeField] private Sprite[] possibleSprites;
    private Sprite lastSprite;
    private Image image;
    [SerializeField] private Animator anim;
    [SerializeField] private float additiveScale;

    private void Awake()
    {
        image = GetComponent<Image>();

        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;
    }

    private Sprite GetNextImage()
    {
        Sprite sprite = RandomHelper.GetRandomFromArray(possibleSprites);
        if (sprite == lastSprite)
            return GetNextImage();
        else
        {
            lastSprite = sprite;
            return sprite;
        }
    }

    public void SetBloodSpatterSprite()
    {
        image.transform.localScale = Vector3.one * RandomHelper.RandomFloat(1, 1 + additiveScale);
        image.transform.localEulerAngles = Vector3.forward * RandomHelper.RandomFloat(0, 360);
        image.sprite = GetNextImage();
    }

    public void CallSpatter()
    {
        anim.SetTrigger("Play");
    }
}
