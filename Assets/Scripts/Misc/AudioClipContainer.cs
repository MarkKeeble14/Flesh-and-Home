using UnityEngine;

[System.Serializable]
public class AudioClipContainer
{
    [SerializeField] private AudioClip clip;
    public AudioClip Clip => clip;
    [SerializeField] private float volume = 1;
    public float Volume => volume;
    [SerializeField] private float pitch = 1;
    public float Pitch => pitch;

    public void PlayOneShot(AudioSource source)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }
}
