using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    // for easy access from other scripts
    public static GameManager Instance;

    private int total_slots = 8;
    private int correctly_placed_planets = 0;
    private bool game_won = false;
    private bool _is_hint_active = false;

    [Header("UI References")]
    public GameObject WinPanel;

    public GameObject GameInfoPanel;
    public TMP_Text winText;
    public GameObject top_info_panel;


        [Header("Game Objects")]
        public Drag_n_drop[] allPlanets;
        public ItemSlot[] allSlots;

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

        if (Global_Audio_Manager.Instance != null)
            Global_Audio_Manager.Instance.Play_Win_SFX();
        else
            Debug.LogError("Global Audio Manager is null!!");


    }

    // Load the main menu screen
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Default Screen");
    }

    public void Start_Game()
    {
        if (GameInfoPanel != null)
            GameInfoPanel.SetActive(false);
        else
            Debug.LogError("Info Panel is null!");

        // Audio_Manager.Instance.Play_Music();
        top_info_panel.gameObject.SetActive(false);
    }

    // Highlight one umatched planet to an unmatched slot for 2 seconds
    public void Show_Hint()
    {
        // dont show hint if there is already one active or the game is over
        if (_is_hint_active || game_won)
            return;
        else
            StartCoroutine(Hint_Coroutine());
    }
    
    // Coroutine for the hint funtion so it runs in parallel
    private IEnumerator Hint_Coroutine()
    {
        _is_hint_active = true;
        Drag_n_drop target_planet = null;
        ItemSlot target_slot = null;

        // Find a planet not yet placed correctly
        if (allPlanets != null)
        {
            foreach (var planet in allPlanets)
            {
                if (planet == null)
                    continue;

                if (!planet.Is_Correctly_Placed())
                {
                    target_planet = planet;

                    // find a slot that matches the planets order
                    if (allSlots != null)
                    {
                        foreach (var slot in allSlots)
                        {
                            if (slot != null && slot.expectedOrder == target_planet.data.orderFromSun)
                            {
                                target_slot = slot;
                                break;
                            }
                        }
                    }
                    if (target_slot != null)
                        break;                  // we found a match
                }
            }
        }


        if (target_planet != null && target_slot != null)
        {
            Debug.Log($"Hint: Move {target_planet.data.planetName} to Slot {target_slot.expectedOrder}");

            // highlight them
            target_planet.Highlight(true);
            target_slot.Highlight(true);

            // wait 
            yield return new WaitForSeconds(2.0f);

            // turn off highlight
            target_planet.Highlight(false);
            target_slot.Highlight(false);

        }
        else
        {
            Debug.Log("No hint available - all planets placed");
        }
        _is_hint_active = false;
    }

}
