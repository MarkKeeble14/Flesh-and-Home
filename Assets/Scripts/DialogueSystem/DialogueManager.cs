using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _Instance { get; private set; }

    [Tooltip("This should have the same number of items as the Affiliation enum.")]
    [SerializeField] private SerializableDictionary<Affiliation, Color> affColorDictionary = new SerializableDictionary<Affiliation, Color>();
    [SerializeField] private TextAsset locFile;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private AudioSource speakerSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private Dialogue onStartDialogue;
    private Coroutine currentDialogueCoroutine;

    [SerializeField] private AudioClipContainer onSnippetPlay;
    [SerializeField] private AudioClipContainer onCharPlay;
    [SerializeField] private float afterSnippetWaitTime = 1.5f;
    [SerializeField] private bool byChar;
    private int numCoroutinesActive;
    public bool Idle => numCoroutinesActive == 0;

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;
    }

    private void Start()
    {
        dynamicVariableDict.Add("gameDuration", delegate
        {
            return Mathf.RoundToInt((GameManager._Instance.GetGameDuration() % 60)).ToString() + " minutes";
        });
        onStartDialogue.Play();
    }

    private Dictionary<string, Func<string>> dynamicVariableDict = new Dictionary<string, Func<string>>();
    private char variableDelimiterStart = '{';
    private char variableDelimiterEnd = '}';

    public string GetDynamicVariable(string key)
    {
        if (dynamicVariableDict.ContainsKey(key))
        {
            return dynamicVariableDict[key]?.Invoke();
        }
        throw new Exception("Missing Variable Name as Key in Dynamic Variable Dictionary for key = " + key);
    }

    public void PlayDialogue(List<string> lineIds)
    {
        List<DialogueSnippet> dialogue = new List<DialogueSnippet>();

        // Fetch Data
        foreach (string id in lineIds)
        {
            dialogue.Add(DialogueSnippet.GetSnippetFromLocJSON(id, locFile));
        }

        // Interrupt current dialogue coroutine if need be 
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            numCoroutinesActive--;
        }

        // Set the current coroutine
        currentDialogueCoroutine = StartCoroutine(ExecuteDialogue(dialogue));
    }

    // Needs to be called as a coroutine (which it is).
    // Will play the snippets in turn to their full length, and finish when they are all done.
    private IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        numCoroutinesActive++;
        foreach (DialogueSnippet ds in snippets)
        {
            //Set subtitle text.
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

            //Rack and play the audio clip.
            speakerSource.clip = ds.speechClip;
            speakerSource.Play();

            if (byChar)
            {
                string toShow = FillInDynamicVariables(ds.text);
                float timeBetweenChars = ds.speakTime / toShow.Length;
                targetText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(speakerColor) + ">" +
                    ds.speaker + ": </color>";

                int index = 0;
                for (int i = 0; i < toShow.Length; i++)
                {
                    char c = toShow[i];
                    // Debug.Log("Char c = " + c);
                    index++;
                    targetText.text += c;
                    onCharPlay.PlayOneShot(sfxSource);
                    yield return new WaitForSeconds(timeBetweenChars);
                }

                //Wait the required time as per snippet description -
                //useful for cutting lines off or something like that.
                yield return new WaitForSeconds(afterSnippetWaitTime);
            }
            else
            {
                targetText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(speakerColor) + ">" +
        ds.speaker + ": </color>" + FillInDynamicVariables(ds.text);
                onSnippetPlay.PlayOneShot(sfxSource);

                yield return new WaitForSeconds(ds.speakTime);
            }

            //Clear subtitles after done talking.
            targetText.text = "";
        }

        currentDialogueCoroutine = null;
        numCoroutinesActive--;
    }

    private string FillInDynamicVariables(string s)
    {
        if (!s.Contains(variableDelimiterStart) || !s.Contains(variableDelimiterEnd)) return s;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            // Debug.Log("Char = " + c);

            if (c.Equals(variableDelimiterStart))
            {
                // Debug.Log("Found Dynamic Variable");

                string variableName = "";
                for (int k = i + 1; ; k++)
                {
                    if (s[k].Equals(variableDelimiterEnd))
                    {
                        // Debug.Log("End Dynamic Variable");
                        string before = s.Substring(0, i - variableName.Length);
                        string filledIn = GetDynamicVariable(variableName);
                        string after = s.Substring(i + 2, s.Length - i - 2);
                        s = before + filledIn + after;
                        // Debug.Log("Before: " + before + "\nFilled :" + filledIn + "\nAfter: " + after + "\nReturning: " + s);
                        return FillInDynamicVariables(s);
                    }
                    // Debug.Log("Dynamic Variable Name (Working) = " + variableName + ", Current Char = " + s[k]);
                    i++;
                    variableName += s[k];
                }
            }
        }
        return s;
    }
}
