using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    // for easy access from other scripts
    public static GameManager Instance;

    private int total_slots = 8;
    private int correctly_placed_planets = 0;
    private bool game_won = false;

    [Header("UI References")]
    public GameObject WinPanel;

    public GameObject InfoPanel;
    public TMP_Text winText;
    public Button button;   // return to main menu button

    void Awake()
    {
        // Setup Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Counts correctly placed planets
    // Calls WinGame() when the game is solved
    public void OnPlanetLocked(int expectedOrder)
    {
        if (game_won)
            return;

        correctly_placed_planets++;
        Debug.Log($"Planet locked in slot {expectedOrder}. Total correct: {correctly_placed_planets}/{total_slots}");

        //check win condition
        if (correctly_placed_planets >= total_slots)
            WinGame();
    }

    // Show winning screen
    private void WinGame()
    {
        game_won = true;
        Debug.Log("All planets are in order!");
        InfoDisplay_TMP.Instance.HideInfo();    // hide info for the winning screen

        //enable pop up screen
        if (WinPanel != null)
            WinPanel.SetActive(true);
        else
            Debug.LogError("Win Panel is null!!!");

        if (Audio_Manager.Instance != null && Audio_Manager.Instance.game_win_sfx != null)
            Audio_Manager.Instance.PlayGameWin_SFX();
        else
            Debug.LogError("Audio Manager or game_win SFX is null!!");


    }

    // Load the main menu screen / go to next level
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Default Screen");
    }
    
    public void Start_Game()
    {
        if (InfoPanel != null)
            InfoPanel.SetActive(false);
        else
            Debug.LogError("Info Panel is null!");

        Audio_Manager.Instance.Play_Music();
    }
}
