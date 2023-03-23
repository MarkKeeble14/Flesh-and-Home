using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [SerializeField] List<string> lineIds = new List<string>();

    public void Play()
    {
        // Play Data
        DialogueManager._Instance.PlayDialogue(lineIds);
    }
}
