using UnityEngine;

public class Global_Audio_Manager : MonoBehaviour
{
    // Singleton wste na mporoume na ton kaloume apo pantou
    public static Global_Audio_Manager Instance;
    [Header("Audio Sources")]
    // Source for background music
    [SerializeField] private AudioSource music_source;
    // Source for one-shot sound effects
    [SerializeField] private AudioSource sfx_source;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // for staying alive between scenes
        }
        else
        {
            Destroy(gameObject); // dont create duplicate
        }
    }

    // for background music
    public void Play_Music(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            music_source.Stop();
            Debug.LogError("Audio clip for background music is null");
            return;
        }
        // if the same music was playing dont change it
        if (music_source.clip == clip && music_source.isPlaying)
        {
            music_source.volume = volume;
            return;
        }

        // Play the music
        music_source.clip = clip;
        music_source.volume = volume;
        music_source.loop = true;
        music_source.Play();
    }

    private void Play_SFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            Debug.LogError("SFX clip is null");
        else
            sfx_source.PlayOneShot(clip, volume);
    }

    public void Play_SFX_correct()
    {
        Play_SFX(Scene_Audio.Instance.sfx_correct,Scene_Audio.Instance.volume);
    }
    public void Play_Win_SFX()
    {
        Play_SFX(Scene_Audio.Instance.sfx_win,Scene_Audio.Instance.volume);
    }
    public void Play_Error_SFX()
    {
        Play_SFX(Scene_Audio.Instance.sfx_error,Scene_Audio.Instance.volume);
    }
}
