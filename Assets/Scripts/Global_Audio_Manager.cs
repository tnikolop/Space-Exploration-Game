using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Global_Audio_Manager : MonoBehaviour
{
    // Singleton wste na mporoume na ton kaloume apo pantou
    public static Global_Audio_Manager Instance;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource music_source;          // Source for background music
    [SerializeField] private AudioSource sfx_source;            // Source for one-shot sound effects

    [Header("UI Settings")]
    [SerializeField] private GameObject settings_panel;
    [SerializeField] private Slider music_slider;
    [SerializeField] private Slider sfx_slider;
    [SerializeField] private TextMeshProUGUI sfx_slider_text;
    [SerializeField] private TextMeshProUGUI music_slider_text;

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

    void Start()
    {
        if (settings_panel != null)
        {
            settings_panel.SetActive(false);

            if (music_slider != null)
                music_slider.value = music_source.volume;
            else
                Debug.LogError("music slider is null");

            if (sfx_slider != null)
                sfx_slider.value = sfx_source.volume;
            else
                Debug.LogError("sfx slider is null");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle_Settings();
        }
    }

    public void Toggle_Settings()
    {
        if (settings_panel == null)
        {
            Debug.LogError("Settings Panel is null");
            return;
        }

        bool is_active = settings_panel.activeSelf;
        settings_panel.SetActive(!is_active);   // toggle on/off
    }

    public void Quit_Game()
    {
        Application.Quit();
    }

    public void Set_Music_Volume(float volume)
    {
        music_source.volume = volume;
        music_slider_text.text = volume.ToString("F2");
    }

    public void Set_SFX_Volume(float volume)
    {
        sfx_source.volume = volume;
        sfx_slider_text.text = volume.ToString("F2");
    }

    public void Play_Music(AudioClip clip)
    {
        if (music_slider.value == 0)
            music_slider.value = 1;
        Play_Music(clip, music_slider.value);
    }
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
            // update slider
            if (music_slider != null)
                music_source.volume = music_slider.value;
            music_source.volume = volume;
            return;
        }

        // Play the music
        music_source.clip = clip;
        music_source.volume = volume;
        // update slider
        if (music_slider != null)
            music_slider.value = volume;
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
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Correct(), Scene_Audio.Instance.Get_Volume());
    }
    public void Play_Win_SFX()
    {
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Win(), Scene_Audio.Instance.Get_Volume());
    }
    public void Play_Error_SFX()
    {
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Error(), Scene_Audio.Instance.Get_Volume());
    }
    public void Play_Click_SFX()
    {
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Click(), Scene_Audio.Instance.Get_Volume());
    }
    public void Play_Game_Completed_SFX()
    {
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Game_Completed(), Scene_Audio.Instance.Get_Volume());

    }
    public void Play_Notification_SFX()
    {
        Play_SFX(Scene_Audio.Instance.GetAudio_SFX_Notification(), Scene_Audio.Instance.Get_Volume());

    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Default Screen");
    }
}
