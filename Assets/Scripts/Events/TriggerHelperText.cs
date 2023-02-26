using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TriggerHelperText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPrefab;
    [SerializeField] private Transform textHolder;
    private Dictionary<TextPromptKeyTrigger, TextMeshProUGUI> spawnedTexts = new Dictionary<TextPromptKeyTrigger, TextMeshProUGUI>();

    public void Show(TextPromptKeyTrigger trigger, string text)
    {
        if (spawnedTexts.ContainsKey(trigger)) return;
        TextMeshProUGUI spawned = Instantiate(textPrefab, textHolder);
        spawnedTexts.Add(trigger, spawned);
        spawned.text = text;
    }

    public void Hide(TextPromptKeyTrigger trigger)
    {
        if (!spawnedTexts.ContainsKey(trigger)) return;
        Destroy(spawnedTexts[trigger].gameObject);
        spawnedTexts.Remove(trigger);
    }
}
