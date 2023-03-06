using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthTrigger : TextPromptKeyTrigger
{
    [SerializeField] private FloatStore hpStore;
    [SerializeField] private float hpRestore;

    private new void Awake()
    {
        onActivate += () => hpStore.AlterFloat(hpRestore);
        base.Awake();
    }
}
