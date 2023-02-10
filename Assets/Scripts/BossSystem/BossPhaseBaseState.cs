using System.Collections;
using UnityEngine;

public abstract class BossPhaseBaseState : MonoBehaviour
{
    [Header("Phase")]
    [SerializeField] protected float enterPhaseTime = 2.5f;

    [Header("Bar Settings")]
    [SerializeField] protected ImageSliderBarSettings phaseBarSettings;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onEnterPhaseClip;
    [SerializeField] private AudioClipContainer onExitPhaseClip;

    public virtual void EnterState(BossPhaseManager boss)
    {
        // Audio
        onEnterPhaseClip.PlayOneShot(boss.source);

        // Start Phase Behaviour
        StartCoroutine(StateBehaviour(boss));

        // Set Bar Settings
        boss.HPBar.SetFromSettings(phaseBarSettings);
    }

    protected abstract IEnumerator StateBehaviour(BossPhaseManager boss);
    public virtual void UpdateState(BossPhaseManager boss)
    {
        // 
    }
    public virtual void ExitState(BossPhaseManager boss)
    {
        // Disable HP Bar
        boss.HPBar.gameObject.SetActive(false);

        // Audio
        onExitPhaseClip.PlayOneShot(boss.source);
    }
}