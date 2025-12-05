using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu_manager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject LevelSelectPanel;
    [SerializeField] private GameObject GameCompletedPanel;
    [SerializeField] private GameObject AchievementPanel;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button[] LevelButtons;
    [SerializeField] private GameObject[] LevelStars;
    [SerializeField] private Image[] AchievementStars;
    private const int _NUM_of_LEVELS = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelSelectPanel.SetActive(false);
        GameCompletedPanel.SetActive(false);
        AchievementPanel.SetActive(false);
        if (Saved_Data_Exists())
            ResumeButton.gameObject.SetActive(true);
        else
            ResumeButton.gameObject.SetActive(false);
    }

    // Start New Game with clean data
    public void New_Game()
    {
        Delete_Saved_Data();
        Load_Game();
    }

    // Start Game with saved Data
    public void Load_Game()
    {
        Lock_buttons();
        Check_Hard_Mode();
        LevelSelectPanel.SetActive(true);
    }

    // Load selected Level
    public void OpenLevel(int level_id)
    {
        SceneManager.LoadScene("Level " + level_id);
    }

    // locks the levels that are not available yet
    private void Lock_buttons()
    {
        // get the current level reached, default 1 if it doesnt exist yet
        int level_reached = PlayerPrefs.GetInt("Level-Reached", 1);
        // Debug.Log("Lock_buttons() Level reached: " + level_reached);

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i + 1 > level_reached)    // lock next levels
            {
                LevelButtons[i].interactable = false;
                LevelButtons[i].image.color = Color.gray;
            }
        }
    }

    // Check if any level has been completed on hard mode and highlight them
    private void Check_Hard_Mode()
    {
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            int hard_mode = PlayerPrefs.GetInt("No-Hint-Level" + (i + 1), 0);
            if (hard_mode == 0)
                LevelStars[i].SetActive(false);
            else
            {    // highlight
                LevelStars[i].SetActive(true);
            }
        }
    }

    // returns true if saved Data exists for this computer
    private bool Saved_Data_Exists()
    {
        return PlayerPrefs.HasKey("Level-Reached");
    }

    // Delete all saved data
    private void Delete_Saved_Data()
    {
        PlayerPrefs.DeleteAll();
    }

    // Show achievement Panel
    public void Show_Achievements()
    {
        AchievementPanel.SetActive(true);
        // Reset all achievements, make them black
        foreach (var star in AchievementStars)
        {
            star.color = Color.black;
        }

        // Check achievemnt 1
        int res = PlayerPrefs.GetInt("Level-Reached", 1);
        if (res > _NUM_of_LEVELS)
        {
            // all levels completed: Achievement 1 unlocked
            AchievementStars[0].color = Color.white;
        }

        // Check achievement 2
        // res = PlayerPrefs.GetInt("Total Stars", 0);
        // if (res >= _NUM_of_LEVELS)
        // {
        //     // all Stars collected: Achievement 2 unlocked
        //     AchievementStars[1].color = Color.white;
        // }
        res = 0;
        for (int i = 1; i < _NUM_of_LEVELS + 1; i++)
        {
            res += PlayerPrefs.GetInt("No-Hint-Level" + i, 0);
        }
        if (res >= _NUM_of_LEVELS)
        {
            // all Stars collected: Achievement 2 unlocked
            AchievementStars[1].color = Color.white;
        }
        // Chech achievement 3
    }


}
