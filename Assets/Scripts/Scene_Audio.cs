using UnityEngine;

public class Scene_Audio : MonoBehaviour
{
    [Header("Audio Clips for this scene")]
    [SerializeField] private AudioClip _background_music;
    [SerializeField] private AudioClip _sfx_correct;
    [SerializeField] private AudioClip _sfx_error;
    [SerializeField] private AudioClip _sfx_win;
    [SerializeField] private AudioClip _sfx_click;
    [SerializeField] private AudioClip _sfx_game_completed;


    [SerializeField] [Range(0f, 1f)] private float _volume = 1f;

    // Singleton wste na mporoume na ton kaloume apo pantou
    public static Scene_Audio Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // dont create duplicate
    }

    void Start()
    {
        if (Global_Audio_Manager.Instance != null)
            Global_Audio_Manager.Instance.Play_Music(_background_music);
        else
            Debug.LogError("Global_Audio_Manager Instance is null");
    }

    public AudioClip GetAudio_SFX_Click()
    {
        if (_sfx_click == null)
            Debug.LogError("SFX_CLICK IS NULL FOR THIS SCENE!");
        return _sfx_click;
    }

    public AudioClip GetAudio_SFX_Win()
    {
        if (_sfx_win == null)
            Debug.LogError("SFX_Win IS NULL FOR THIS SCENE!");
        return _sfx_win;
    }

    public AudioClip GetAudio_SFX_Error()
    {
        if (_sfx_error == null)
            Debug.LogError("SFX_Error IS NULL FOR THIS SCENE!");
        return _sfx_error;
    }
    public AudioClip GetAudio_SFX_Correct()
    {
        if (_sfx_correct == null)
            Debug.LogError("SFX_Correct IS NULL FOR THIS SCENE!");
        return _sfx_correct;
    }
    public AudioClip GetAudio_SFX_Game_Completed()
    {
        if (_sfx_game_completed == null)
            Debug.LogError("SFX_Game_Completed IS NULL FOR THIS SCENE!");
        return _sfx_game_completed;
    }
    public AudioClip GetAudio_Background_Music()
    {
        if (_background_music == null)
            Debug.LogError("Background Music IS NULL FOR THIS SCENE!");
        return _background_music;
    }

    public float Get_Volume()
    {
        return _volume;
    }
}
