using System.Collections;
using UnityEngine;

public class BossBarrel : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer
    {
        get { return lineRenderer; }
    }

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onStartClip;
    [SerializeField] private AudioClipContainer onEndClip;
    [SerializeField] private AudioSource constantSource;
    [SerializeField] private AudioSource source;

    [Header("Scale")]
    [SerializeField] private Vector3 barrelScalePerAxis = Vector3.one;
    [SerializeField] private Vector3 barrelTranslateScalePerAxis = Vector3.one;

    [SerializeField] private new Rigidbody rigidbody;
    public Rigidbody Rigidbody => rigidbody;

    private void Awake()
    {
        // Scale 
        transform.localScale = new Vector3(
            transform.localScale.x * barrelScalePerAxis.x,
            transform.localScale.y * barrelScalePerAxis.y,
            transform.localScale.z * barrelScalePerAxis.z);

        // Transform 
        transform.localPosition = new Vector3(
            transform.localPosition.x * barrelTranslateScalePerAxis.x,
            transform.localPosition.y * barrelTranslateScalePerAxis.y,
            transform.localPosition.z * barrelTranslateScalePerAxis.z);
    }

    private bool isAttached = true;
    public bool IsAttached
    {
        get
        {
            return isAttached;
        }
        set
        {
            isAttached = value;
        }
    }

    private bool isFiring;
    public bool IsFiring
    {
        get
        {
            return isFiring;
        }
        set
        {
            // if not currently firing but incoming value will change barrel to be firing, the barrel is presumed to be starting to fire
            if (!isFiring && value)
            {
                onStartClip.PlayOneShot(source);
            }
            if (isFiring && !value) // Opposite to check when we're turning off the barrel
            {
                onEndClip.PlayOneShot(source);
            }
            isFiring = value;
        }
    }

    private void Update()
    {
        // Audio
        constantSource.enabled = isFiring;
    }
}
