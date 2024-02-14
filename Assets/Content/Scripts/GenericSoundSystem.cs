using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSoundSystem : MonoBehaviour
{
    #region Singleton
    public static GenericSoundSystem instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip, float _volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = _volume;
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySFXAtPosition(AudioClip clip, float _volume, Vector3 position)
    {
        if (audioSource != null)
        {
            audioSource.volume = _volume;
            audioSource.PlayOneShot(clip);
        }
    }
}
