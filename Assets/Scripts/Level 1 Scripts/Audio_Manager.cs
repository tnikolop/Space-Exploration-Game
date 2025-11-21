using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager Instance;   // Singleton
    
    [Header("Audio Sources")]
    // Source for background music
    [SerializeField] private AudioSource music_source; 
    // Source for one-shot sound effects
    [SerializeField] private AudioSource sfx_source; 

    [Header("Audio Clips")]
    public AudioClip background_music;
    // Sound effect when a planet is placed correctly
    public AudioClip correct_placement_sfx;
    // Sound effect when the game is won
    public AudioClip game_win_sfx;
    public AudioClip error_sfx;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     //Start playhing the background music when the game loads
    //     if (background_music != null && music_source != null)
    //         Play_Music(background_music);
    //     else
    //         Debug.LogWarning("Background music or music source not assigned in AudioManager.");

    // }

    //Plays the background music, setting it to loop.
    public void Play_Music()
    {
        if (music_source != null && background_music != null)
        {
            music_source.clip = background_music;
            music_source.loop = true;
            music_source.Play();
        }
        else
            Debug.LogWarning(" background_music or music source is null.");

    }

    // Plays a sound effect once
    private void Play_SFX(AudioClip clip)
    {
        if (sfx_source != null && clip != null)
        {
            sfx_source.PlayOneShot(clip);
        }
        else
            Debug.LogWarning(" Audio clip or sfx_source is null.");

    }

    // is called when the planet is placed in the correct slot
    public void PlayCorrectPlacement_SFX()
    {
        Play_SFX(correct_placement_sfx);
    }

    // is called when the game is completed
    public void PlayGameWin_SFX()
    {
        Play_SFX(game_win_sfx);
    }

    public void Play_Error_SFX()
    {
        Play_SFX(error_sfx);
    }
}
