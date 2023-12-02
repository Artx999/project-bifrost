using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxSource;

    [Header("Game SFXs")]
    public AudioClip axeThrow;
    public AudioClip axeHit;
    public AudioClip landing;
    public AudioClip backLanding;
    public AudioClip wallLanding;

    [Header("Menu SFXs")]
    public AudioClip buttonHover;
    public AudioClip buttonClick;

    private void Awake()
    {
        //sfxSource = this.AddComponent<AudioSource>();
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
