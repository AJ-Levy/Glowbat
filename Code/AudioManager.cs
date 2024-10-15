using UnityEngine;

/// <summary>
/// Class that manages music and SFX.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("--------- Audio Source ---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip batflying;
    public AudioClip death;
    public AudioClip firefly;
    public AudioClip menu;
    public AudioClip gameOver;
    public AudioClip powerUp;
    public AudioClip shieldBreak;
    public AudioClip milestone;

    /// <summary>
    /// Play the background track.
    /// </summary>
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    /// <summary>
    /// Play sound effects
    /// </summary>
    /// <param name="clip">The SFX clip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play/Resume the music.
    /// </summary>
    public void PlayMusic()
    {
        musicSource.Play();
    }

    /// <summary>
    /// Pause the music.
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }
}
