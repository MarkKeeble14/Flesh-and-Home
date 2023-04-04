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

    private IEnumerator ForceDestroyAfter(GameObject text, float duration)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            yield return null;
        }

        Destroy(text);
    }

    private IEnumerator ExecuteSpawnText(string text, float duration)
    {
        TextMeshProUGUI spawned = Instantiate(onUnlockPopupText, spawnedTextParent);
        spawned.text = text;

        StartCoroutine(ForceDestroyAfter(spawned.gameObject, duration + 5.0f));

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
        }

        spawned.GetComponent<Animator>().SetTrigger("Done");
    }
}
