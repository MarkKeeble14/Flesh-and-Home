using UnityEngine;

public class WaitForWeaponToFireEventTriggerer : EventTriggerer
{
    [SerializeField] private WeaponAttachmentController waitFor;

    protected override bool Condition
    {
        get
        {
            return waitFor.Firing;
        }
    }
}


