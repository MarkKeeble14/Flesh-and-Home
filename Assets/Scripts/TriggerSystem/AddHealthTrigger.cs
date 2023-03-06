using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthTrigger : DestroyTriggerOnActivate
{
    [SerializeField] private FloatStore hpStore;
    [SerializeField] private float hpRestore;

    protected override void Activate()
    {
        hpStore.AlterFloat(hpRestore);
    }
}
