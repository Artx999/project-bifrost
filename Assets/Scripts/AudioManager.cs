using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("GameObject references")]
    public GameObject mainCamera;
    public GameObject gameAudioSource;
    public GameObject pauseAudioSource;
    public GameObject menuAudioSource;
    
    [Header("Game SFXs")]
    public AudioClip axeThrow;
    public AudioClip axeHit;
    public AudioClip landing;
    public AudioClip backLanding;
    public AudioClip wallLanding;

    [Header("Menu SFXs")]
    public AudioClip buttonHover;
    public AudioClip buttonClick;
    
    [Header("Pause SFXs")]
    public AudioClip pauseAndResume;

    private AudioSource _gameAudioSource;
    private AudioSource _pauseAudioSource;
    private AudioSource _menuAudioSource;
    
    private void Awake()
    {
        this.mainCamera = GameObject.FindWithTag("MainCamera");
    }

    private void Start()
    {
        this._gameAudioSource = this.gameAudioSource.GetComponent<AudioSource>();
        this._pauseAudioSource = this.pauseAudioSource.GetComponent<AudioSource>();
        this._menuAudioSource = this.menuAudioSource.GetComponent<AudioSource>();
        
        // The pause menu should not be affected by a pausing game
        this._pauseAudioSource.ignoreListenerPause = true;
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == this.axeThrow || clip == this.axeHit || clip == this.landing 
            || clip == this.backLanding || clip == this.wallLanding)
        {
            if (this._gameAudioSource.isPlaying)
                return;
            this._gameAudioSource.PlayOneShot(clip);
        }
        else if (clip == this.buttonHover || clip == this.buttonClick)
        {
            if (this._menuAudioSource.isPlaying)
                return;
            this._menuAudioSource.PlayOneShot(clip);
        }
        else if (clip == this.pauseAndResume)
        {
            if (this._pauseAudioSource.isPlaying)
                return;
            this._pauseAudioSource.PlayOneShot(clip);
        }
    }

    public void PauseAllAudio(bool shouldPause)
    {
        AudioListener.pause = shouldPause;
    }
}
