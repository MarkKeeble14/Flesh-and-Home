using System.Collections.Generic;

public class WaitForIdleDialogueEventTriggerer : EventTriggerer
{
    protected override bool Condition => DialogueManager._Instance.Idle;
}
