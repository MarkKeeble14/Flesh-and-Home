using Newtonsoft.Json.Linq;
using System;
//using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

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
        iff = Affiliation.NEUTRAL;
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
                result.iff = Affiliation.FRIEND;
                break;
            case "enemy":
                result.iff = Affiliation.ENEMY;
                break;
            default:
                result.iff = Affiliation.NEUTRAL;
                break;
        }

        //Number is also easy.
        result.speakTime = ((float)jsonSnippet["speakTime"]);

        //Using the Resources folder, we can load audio as needed.
        result.speechClip = Resources.Load<AudioClip>(voicelineRoot + "/" + jsonSnippet["speechClip"].ToString());

        return result;
    }
}
