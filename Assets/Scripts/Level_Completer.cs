using UnityEngine;

public class Level_Completer : MonoBehaviour
{
    public static Level_Completer Instance;
    [SerializeField] private int current_level;

    void Awake()
    {
        // Singleton pattern so  any script can call
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    // Mark Level as completed and unlock the next level
    public void WinLevel(bool hard_mode)
    {
        // Get the currently reached level
        int currentLevelReached = PlayerPrefs.GetInt("Level-Reached", 1);
        int next_level_unlock = current_level + 1;

        // Only save if we are unlocking a NEW level, We don't want replaying Level 1 to reset progress back to 2.
        if (next_level_unlock > currentLevelReached)
        {
            PlayerPrefs.SetInt("Level-Reached", next_level_unlock);
            PlayerPrefs.Save(); //write to disk
            Debug.Log("Unlocked Level: " + next_level_unlock);
        }
        
        if (hard_mode)
        {
            WinLevel_Hard(current_level);
        }
    }

    // Mark level as won on hard-mode (no hint used)
    private void WinLevel_Hard(int level)
    {
        PlayerPrefs.SetInt("No-Hint-Level" + level, 1);
        Debug.Log("Level " + level + " won without hint");
        PlayerPrefs.Save();
    }

    // mark current level as comlete and unlock the next one
    // FOR TESTING!!!!!!!!!!!!!!
#if UNITY_EDITOR
    // Αυτό προσθέτει επιλογή στο δεξί κλικ του Component
    [ContextMenu("💾 Unlock all Levels")]
    public void unlock_all()
    {
        int total = current_level;
        for (int i = 0; i < total; i++)
        {
            current_level = i + 1;
            WinLevel(false);

        }
    }
    [ContextMenu("💾 Unlock all Levels Hard")]
    public void unlock_allHard()
    {
        int total = current_level;
        for (int i = 0; i < total; i++)
        {
            current_level = i + 1;
            WinLevel(true);
            
        }
    }
#endif
}