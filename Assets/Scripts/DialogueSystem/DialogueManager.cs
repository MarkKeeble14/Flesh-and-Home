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

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var i in affiliationColorInput)
        {
            affColors[i.aff] = i.color;
        }

        //Look for unassigned objects in this gameobject.
        if (targetTextObject != null)
        {
            targetText = targetTextObject.GetComponent<TextMeshProUGUI>();
        } else
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    //Needs to be called as a coroutine (which it is).
    //Will play the snippets in turn to their full length, and finish when they are all done.
    public IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        foreach(DialogueSnippet ds in snippets)
        {
            //These will get played in turn.
            yield return StartCoroutine(ExecuteSnippet(ds));
        }
    }

    public IEnumerator ExecuteSnippet(DialogueSnippet ds)
    {
        //Set subtitle text.
        SetTextDialogue(ds);

        //Rack and play the audio clip.
        audioSource.clip = ds.speechClip;
        audioSource.Play();

        //Wait the required time as per snippet description -
        //useful for cutting lines off or something like that.
        yield return new WaitForSeconds(ds.speakTime);
        //Clear subtitles after done talking.
        targetText.text = "";
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

        targetText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(speakerColor) + ">" +
            ds.speaker + "</color>" + ": " + ds.text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
