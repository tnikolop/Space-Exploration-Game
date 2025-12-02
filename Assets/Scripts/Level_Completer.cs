using UnityEngine;

public class Level_Completer : MonoBehaviour
{
    public static Level_Completer Instance;
    [SerializeField] private int levelToUnlock;     // set in the editor for each level

    void Awake()
    {
        // Singleton pattern so  any script can call Infodisplay_TMP.Instance.ShowInfo()
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // mark current level as comlete and unlock the next one
#if UNITY_EDITOR
    // Αυτό προσθέτει επιλογή στο δεξί κλικ του Component
    [ContextMenu("💾 Unlock Level")]
    public void WinLevel()
    {
        // Get the currently reached level
        int currentLevelReached = PlayerPrefs.GetInt("Level-Reached", 1);

        // Only save if we are unlocking a NEW level, We don't want replaying Level 1 to reset progress back to 2.
        if (levelToUnlock > currentLevelReached)
        {
            PlayerPrefs.SetInt("Level-Reached", levelToUnlock);
            PlayerPrefs.Save(); //write to disk
            Debug.Log("Unlocked Level: " + levelToUnlock);
        }
    }
#endif
}