using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu_manager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject LevelSelectPanel;
    [SerializeField] private GameObject GameCompletedPanel;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button[] LevelButtons;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelSelectPanel.SetActive(false);
        GameCompletedPanel.SetActive(false);
        // ResumeButton.gameObject.SetActive(false);
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
        LevelSelectPanel.SetActive(true);
    }
    
    // Load selected Level
    public void OpenLevel(int level_id)
    {
        SceneManager.LoadScene("Level " + level_id);
    }

    // locks the levels that are not available yet
    void Lock_buttons()
    {
        // get the current level reached, default 1 if it doesnt exist yet
        int level_reached = PlayerPrefs.GetInt("Level-Reached", 1);

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i + 1 > level_reached)    // lock next levels
            {
                LevelButtons[i].interactable = false;
                LevelButtons[i].image.color = Color.gray;
            }
        }
    }
    
    // Delete all saved data
    private void Delete_Saved_Data()
    {
        PlayerPrefs.DeleteAll();
    }
}
