using System.Collections;
using UnityEngine;
using TMPro;

public class TextPopupManager : MonoBehaviour
{
    public static TextPopupManager _Instance { get; private set; }

    private void Awake()
    {
        _Instance = this;
    }

    [SerializeField] private TextMeshProUGUI onUnlockPopupText;
    [SerializeField] private Transform spawnedTextParent;

    public void SpawnText(string text, float duration)
    {
        StartCoroutine(ExecuteSpawnText(text, duration));
    }

    private IEnumerator ExecuteSpawnText(string text, float duration)
    {
        TextMeshProUGUI spawned = Instantiate(onUnlockPopupText, spawnedTextParent);
        spawned.text = text;
        yield return new WaitForSeconds(duration);
        spawned.GetComponent<Animator>().SetTrigger("Done");
    }
}
