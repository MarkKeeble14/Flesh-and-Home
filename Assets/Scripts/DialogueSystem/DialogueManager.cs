using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Serializable]
    public struct AffColor
    {
        public Affiliation aff;
        public Color color;
    }

    //I can't figure out a way to force it to be the correct size.
    //Dictionaries can't be serialized, so we'll do it this way.
    [Tooltip("This should have the same number of items as the Affiliation enum.")]
    public List<AffColor> affiliationColorInput = new List<AffColor>();
    Dictionary<Affiliation, Color> affColors= new Dictionary<Affiliation, Color>();


    public GameObject targetTextObject;
    TextMeshProUGUI targetText;

    public TextAsset locFile;

    float textClearTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var i in affiliationColorInput)
        {
            affColors[i.aff] = i.color;
        }
        targetText = targetTextObject.GetComponent<TextMeshProUGUI>();
    }
    
    void SetTextDialogue(DialogueSnippet ds)
    {
        Color speakerColor;

        try {
            speakerColor = affColors[ds.iff];
        } catch (Exception e)
        {
            speakerColor = Color.white;
            Debug.LogWarning("Incomplete affiliation color dictionary in DialogueManager: " + e.Message);
        }

        textClearTimer = ds.speakTime;

        targetText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(speakerColor) + ">" +
            ds.speaker + "</color>" + ": " + ds.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
