using System.Collections;
using UnityEngine;

public class WaitForIdleDialogueWithDelayEventTriggerer : EventTriggerer
{
    [SerializeField] private float delay;
    private bool triggered;
    private bool initiated;

    protected override bool Condition => triggered;

    private void Update()
    {
        if (initiated) return;

        if (DialogueManager._Instance.Idle)
        {
            StartCoroutine(Trigger());
        }
    }

    private IEnumerator Trigger()
    {
        initiated = true;
        yield return new WaitForSeconds(delay);
        triggered = true;
    }
}
