using UnityEngine;

public class Scene_Audio : MonoBehaviour
{
    [Header("Audio Clips for this scene")]
    public AudioClip background_music;
    public AudioClip sfx_correct;
    public AudioClip sfx_error;
    public AudioClip sfx_win;
    [Range(0f, 1f)] public float volume = 1f;

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
            Global_Audio_Manager.Instance.Play_Music(background_music, volume);
        else
            Debug.LogError("Global_Audio_Manager Instance is null");
    }
}
