using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    //plays sound effects
    public AudioSource PlaySFX(AudioClip audioClip, float vol = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = vol;
        audioSource.Play();
        StartCoroutine(DestroyAudioSource(audioSource));
        return audioSource;

    }

    //used for sprinting sfx
    public AudioSource PlayQuickSFX(AudioClip audioClip, float vol = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = vol;
        audioSource.pitch = 1.5f;
        audioSource.Play();
        StartCoroutine(DestroyAudioSource(audioSource));
        return audioSource;

    }

    //destroys audio source
    private IEnumerator DestroyAudioSource(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        if(audioSource != null)
        {
            Destroy(audioSource);
        }
    }
}
