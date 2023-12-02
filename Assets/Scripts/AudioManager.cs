using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Game SFXs")]
    public AudioClip axeThrow;
    public AudioClip axeHit;
    public AudioClip landing;
    public AudioClip backLanding;
    public AudioClip wallLanding;

    [Header("Menu SFXs")]
    public AudioClip buttonHover;
    public AudioClip buttonClick;

    private AudioSource _audioSource;

    private void Start()
    {
        this._audioSource = GetComponent<AudioSource>();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (this._audioSource.isPlaying)
            return;

        this._audioSource.PlayOneShot(clip);
    }
}
