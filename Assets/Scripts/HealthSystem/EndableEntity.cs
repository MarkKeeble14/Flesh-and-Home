using System;
using UnityEngine;


public abstract class EndableEntity : MonoBehaviour, IDamageable
{
    [Header("Audio")]
    [SerializeField] protected AudioSource source;
    [SerializeField] protected TemporaryAudioSource tempSource;
    [SerializeField] protected AudioClipContainer onEndClip;
    protected Action onEndAction;

    public void CallOnEndAction()
    {
        onEndAction?.Invoke();
    }

    public void AddAdditionalOnEndAction(Action action)
    {
        onEndAction += action;
    }

    protected void Awake()
    {
        onEndAction += OnEnd;
    }

    protected virtual void OnEnd()
    {
        // Debug.Log("On End From Endable Entity");

        // Audio
        tempSource.Play(onEndClip);
    }

    public abstract void Damage(float damage, DamageSource source);
    public abstract void Damage(float damage, Vector3 force, DamageSource source);
}
