using UnityEngine;

[System.Serializable]
public class AudioClipContainer
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float volume = 1;
    [SerializeField] private float pitch = 1;

    public void PlayOneShot(AudioSource source)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }
}
