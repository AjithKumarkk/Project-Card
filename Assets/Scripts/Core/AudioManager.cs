using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;

    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip misMatchClip;
    public AudioClip gameCompleteClip;

    void Awake()
    {
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayFlip()
    {
        if (flipClip) source.PlayOneShot(flipClip);
    }

    public void PlayMatch()
    {
        if (matchClip) source.PlayOneShot(matchClip);
    }

    public void PlayMismatch()
    {
        if (misMatchClip) source.PlayOneShot(misMatchClip);
    }

    public void PlayGameComplete()
    {
        if (gameCompleteClip) source.PlayOneShot(gameCompleteClip);
    }
}
