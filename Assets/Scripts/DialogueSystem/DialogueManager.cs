using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class DialogueManager : MonoBehaviour
{
    public static DialogueManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;

        // Set References
        source = GetComponent<AudioSource>();
    }

    [Tooltip("This should have the same number of items as the Affiliation enum.")]
    [SerializeField] private SerializableDictionary<Affiliation, Color> affColorDictionary = new SerializableDictionary<Affiliation, Color>();
    [SerializeField] private TextAsset locFile;
    [SerializeField] private TextMeshProUGUI targetText;
    private AudioSource source;

    public void PlayDialogue(List<string> lineIds)
    {
        List<DialogueSnippet> dialogue = new List<DialogueSnippet>();

        // Fetch Data
        foreach (string id in lineIds)
        {
            dialogue.Add(DialogueSnippet.GetSnippetFromLocJSON(id, locFile));
        }

        StartCoroutine(ExecuteDialogue(dialogue));
    }

    // Needs to be called as a coroutine (which it is).
    // Will play the snippets in turn to their full length, and finish when they are all done.
    private IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        foreach (DialogueSnippet ds in snippets)
        {
            //These will get played in turn.
            yield return StartCoroutine(ExecuteSnippet(ds));
        }
    }

    private IEnumerator ExecuteSnippet(DialogueSnippet ds)
    {
        //Set subtitle text.
        SetTextDialogue(ds);

        //Rack and play the audio clip.
        source.clip = ds.speechClip;
        source.Play();

        //Wait the required time as per snippet description -
        //useful for cutting lines off or something like that.
        yield return new WaitForSeconds(ds.speakTime);

        //Clear subtitles after done talking.
        targetText.text = "";
    }

    void SetTextDialogue(DialogueSnippet ds)
    {
        Color speakerColor;

        try
        {
            speakerColor = affColorDictionary[ds.iff];
        }
        catch (Exception e)
        {
            speakerColor = Color.white;
            Debug.LogWarning("Incomplete affiliation color dictionary in DialogueManager: " + e.Message);
        }

        targetText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(speakerColor) + ">" +
            ds.speaker + "</color>" + ": " + ds.text;
    }
}
