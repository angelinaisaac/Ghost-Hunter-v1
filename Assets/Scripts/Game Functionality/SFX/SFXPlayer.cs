using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] public AudioSource src;
    public static SFXPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    //plays soundeffect
    public void PlaySFX(AudioClip clip)
    {
        src.PlayOneShot(clip);
    }

    //play sound effect with customisable volume
    public void PlaySFXVol(AudioClip clip, float vol)
    {
        src.PlayOneShot(clip, vol);
    }
}
