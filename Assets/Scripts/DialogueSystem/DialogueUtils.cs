using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Rendering.UI;

public enum Affiliation
{
    Friend,
    Neutral,
    Enemy
}

[Serializable]
public class DialogueSnippet
{
    public string speaker;
    public string text;
    public Affiliation iff;
    public float speakTime;
    public AudioClip speechClip;

    public DialogueSnippet()
    {
        speaker = string.Empty;
        text = string.Empty;
        iff = Affiliation.Neutral;
        speakTime = 1;
        speechClip = null;
    }

    public static DialogueSnippet GetSnippetFromLocJSON(string id, TextAsset locJSON)
    {
        //Path to directory within a Resources folder where all the subdirectories containing
        //actual lines are. There needs to be a better way to go about this.
        string voicelineRoot = "SpeechLines";

        DialogueSnippet result = new DialogueSnippet();

        //Load the relevant object from the localization JSON by ID.
        JObject jsonSnippet = (JObject)JObject.Parse(locJSON.text)[id];

        //Debug.Log(jsonSnippet);

        //Text values are easy.
        result.speaker = jsonSnippet["speaker"].ToString();
        result.text = jsonSnippet["text"].ToString();

        //Affiliation enum values are determined by switch case.
        switch (jsonSnippet["iff"].ToString())
        {
            case "friend":
                result.iff = Affiliation.Friend; 
                break;
            case "enemy":
                result.iff = Affiliation.Enemy;
                break;
            default:
                result.iff = Affiliation.Neutral;
                break;
        }

        //Number is also easy.
        result.speakTime = ((float)jsonSnippet["speakTime"]);

        //Using the Resources folder, we can load audio as needed.
        result.speechClip = Resources.Load<AudioClip>(voicelineRoot + "/" + jsonSnippet["speechClip"].ToString());

        return result;
    }

}

public class DialogueLoader
{
    static DialogueLoader instance;

    private DialogueLoader() {}

    public static DialogueLoader GetInstance()
    {
        if (instance == null)
        {
            instance = new DialogueLoader();
        }
        return instance;
    }
}

public class DialogueUtils : MonoBehaviour
{
    public List<string> lineIDs = new List<string>();
    public TextAsset loc;

    public List<DialogueSnippet> snippets = new List<DialogueSnippet>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var l in lineIDs)
        {
            snippets.Add(DialogueSnippet.GetSnippetFromLocJSON(l, loc));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
