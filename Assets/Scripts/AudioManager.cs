using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("GameObject references")]
    public GameObject mainCamera;
    public GameObject gameAudioSource;
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
    public AudioClip pauseAndResume;
    
    private AudioSource _gameAudioSource;
    private AudioSource _menuAudioSource;
    
    private void Awake()
    {
        this.mainCamera = GameObject.FindWithTag("MainCamera");
    }

    private void Start()
    {
        this._gameAudioSource = this.gameAudioSource.GetComponent<AudioSource>();
        this._menuAudioSource = this.menuAudioSource.GetComponent<AudioSource>();
        
        // The pause menu should not be affected by a pausing game
        this._menuAudioSource.ignoreListenerPause = true;
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
        else if (clip == this.buttonHover || clip == this.buttonClick
                 || clip == this.pauseAndResume)
        {
            if (this._menuAudioSource.isPlaying)
                return;
            this._menuAudioSource.PlayOneShot(clip);
        }
    }

    public void PauseAllAudio(bool shouldPause)
    {
        AudioListener.pause = shouldPause;
    }
}
