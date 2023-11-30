using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxSource;

    [Header("SFXs")]
    public AudioClip axeThrow;
    public AudioClip axeHit;
    public AudioClip landing;
    public AudioClip backLanding;
    public AudioClip wallLanding;

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
