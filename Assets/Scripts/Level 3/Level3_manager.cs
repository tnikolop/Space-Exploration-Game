using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Level3_manager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private Transform grid_panel; // The Panel holding the cards
    [SerializeField] private GameObject card_prefab;   // The card object to spawn
    [SerializeField] private int rows = 4;
    [SerializeField] private int cols = 4;
    [SerializeField] private float padding = 20f;     // Space between cards
    [SerializeField] private int edge_padding = 20;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform GameInfoPanel;
    [SerializeField] private Transform InfoPanel;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private Image imageSlot;
    [SerializeField] private Button startScreenButton;

    [Header("Levels Buttons")]
    [SerializeField] private Image[] level_button_image;
    
    [Header("Game Settings")]
    [SerializeField] private float timeToWait = 1.0f; // Time before wrong pair closes in seconds
    [SerializeField] private bool showDebugLogs = false;

    [Header("Data lists")]
    [SerializeField] private List<Card_data> level1_data;
    [SerializeField] private List<Card_data> level2_data;

    private List<Card_data> _current_level_data;
    private Memory_Card _first_card;                // first card flipped open
    private Memory_Card _second_card;               // second card flipped open
    private bool _is_waiting_to_reset = false;      // wait for the clock to finish (cant open new card)
    private float _timer = 0f;                      // the countdown timer for waiting on card reveal
    private bool[] _levels_won = new bool[2];        // true if a level has been won (automatically initialized to false in C#)
    private int _current_level_index;               // current level playing
    private int match_count = 0;                    // how many matches have been found, (win condition check)
    private float _time_elapsed = 0;
    private bool _time_is_running = false;
    private bool _hard_mode = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInfoPanel.gameObject.SetActive(true);
        InfoPanel.gameObject.SetActive(false);
        grid_panel.gameObject.SetActive(false);
        startScreenButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        Highlight_Levels();
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown clock for card reveal
        if (_is_waiting_to_reset)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Close_Mismatch();
            }
        }
        // Actual game timer
        if (_time_is_running)
        {
            _time_elapsed += Time.deltaTime;
            Display_Time(_time_elapsed);
        }
    }

    // Display the the timer on the screen TMP object
    private void Display_Time(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Stop_Timer()
    {
        _time_is_running = false;
        if (showDebugLogs) Debug.Log("Timer stopped at: "+ _time_elapsed);

    }
    private void Start_Timer()
    {
        _time_elapsed = 0;
        _time_is_running = true;
    }

    // Load necessary game data
    public void Select_Level(int level_number)
    {
        _current_level_index = level_number;
        switch (level_number)
        {
            case 0:
                _current_level_data = level1_data;
                cols = 4;
                break;
            case 1:
                _current_level_data = level2_data;
                cols = 5;
                break;
            default:
                Debug.LogError($"Invalid Level Number Selected!: {level_number}");
                return;
        }

        Setup_Level();
    }

    private void Setup_Level()
    {
        GameInfoPanel.gameObject.SetActive(false);
        InfoPanel.gameObject.SetActive(true);
        grid_panel.gameObject.SetActive(true);
        Setup_Grid_Layout();
        Generate_Grid();
        match_count = 0;
        Show_Info(null, null, null);
        imageSlot.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        Start_Timer();
    }

    // function for dynamically setting up the grid layout
    private void Setup_Grid_Layout()
    {
        GridLayoutGroup grid_layout = grid_panel.GetComponent<GridLayoutGroup>();
        if (grid_layout == null)
            if (showDebugLogs) Debug.LogError("Grid Layout Component in Grid Panel does not exists!");

        grid_layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid_layout.constraintCount = cols;
        grid_layout.spacing = new Vector2(padding, padding);
        grid_layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid_layout.childAlignment = TextAnchor.MiddleCenter;

        // setup edge padding
        grid_layout.padding = new RectOffset(edge_padding, edge_padding, edge_padding, edge_padding);

        RectTransform rect = grid_panel.GetComponent<RectTransform>();

        float paddingX = grid_layout.padding.left + grid_layout.padding.right;
        float paddingY = grid_layout.padding.top + grid_layout.padding.bottom;

        // Total width avaialble - padding
        float panel_width = rect.rect.width - paddingX;
        float total_spacingX = (cols - 1) * padding;
        float cell_width = (panel_width - total_spacingX) / cols;

        // Total height avaialble - padding
        float panel_height = rect.rect.height - paddingY;
        float total_spacingY = (rows - 1) * padding;
        float cell_height = (panel_height - total_spacingY) / rows;

        // Pick the smaller one to ensure it fits in both dimensions
        float cellSize = Mathf.Min(cell_width, cell_height);

        // Apply size (keep it square)
        grid_layout.cellSize = new Vector2(cellSize, cellSize);
    }

    // Create the cards
    private void Generate_Grid()
    {
        // Clean old cards
        foreach (Transform child in grid_panel)
            Destroy(child.gameObject);

        int total_cards = rows * cols;
        if (total_cards % 2 != 0)
        {
            if (showDebugLogs) Debug.LogError("Total Cards must be even!");
            return;
        }

        // create pairs
        List<int> card_ids = new List<int>();
        for (int i = 0; i < total_cards / 2; i++)
        {
            card_ids.Add(i);
            card_ids.Add(i);
        }

        // shufle the list (FIsher-yates shuffle)
        for (int i = 0; i < card_ids.Count; i++)
        {
            int temp = card_ids[i];
            int k = Random.Range(i, card_ids.Count);
            card_ids[i] = card_ids[k];
            card_ids[k] = temp;
        }

        //Spawn cards
        foreach (int id in card_ids)
        {
            GameObject card = Instantiate(card_prefab, grid_panel);
            Memory_Card script = card.GetComponent<Memory_Card>();
            card.name = $"Card_{id}";                                   // to onoma pou tha exei ston editor
            script.Setup(id, this, _current_level_data[id]);
        }
    }

    // Cant open a card if we are waiting for the clock
    public bool Can_Flip()
    {
        return !_is_waiting_to_reset;
    }

    // assing card as Face Up
    public void Card_Revealed(Memory_Card card)
    {
        if (_first_card == null)
        {
            _first_card = card;
            if (showDebugLogs) Debug.Log($"Revealed card id:{_first_card.get_ID()}");

        }
        else
        {
            _second_card = card;
            if (showDebugLogs) Debug.Log($"Revealed card id:{_second_card.get_ID()}");
            Check_Match();
        }
    }

    private void Check_Match()
    {
        if (_first_card.get_ID() == _second_card.get_ID())  // match
        {
            if (showDebugLogs) Debug.Log($"Match found for card id:{_first_card.get_ID()}");
            Show_Info();
            _second_card = null;
            _first_card = null;
            Global_Audio_Manager.Instance.Play_SFX_correct();
            match_count++;
            Check_Win();
        }
        else    // Mismatch
        {
            if (showDebugLogs) Debug.Log("Mismatch");
            _timer = timeToWait;
            _is_waiting_to_reset = true;
            Global_Audio_Manager.Instance.Play_Error_SFX();
        }
    }

    private void Check_Win()
    {
        if (match_count >= _current_level_data.Count)
        {
            Global_Audio_Manager.Instance.Play_Win_SFX();
            _levels_won[_current_level_index] = true;
            Save_Progress();
            Highlight_Levels();
            Check_Game_Completed();
            Stop_Timer();

            if (_time_elapsed < 60)     // if under a minute get a star
            {
                _hard_mode = true;
            }
            else
            {
                _hard_mode = false;
            }
        }

    }


    // close the open cards when the time runs out
    private void Close_Mismatch()
    {
        if (_first_card == null)
            if (showDebugLogs) Debug.LogError("_First_Card is null!");
        if (_first_card == null)
            if (showDebugLogs) Debug.LogError("_Second_Card is null!");

        _first_card.Flip_Closed();
        _second_card.Flip_Closed();
        _first_card = null;
        _second_card = null;
        _is_waiting_to_reset = false;
    }


    private void Check_Game_Completed()
    {
        if (_levels_won[0] && _levels_won[1])
        {
            Global_Audio_Manager.Instance.Play_Game_Completed_SFX();
            startScreenButton.gameObject.SetActive(true);
            Level_Completer.Instance.WinLevel(_hard_mode);    // mark level as completed to unlock the next one
        }
    }

    public void Show_Info()
    {
        if (_first_card == null)
        {
            if (showDebugLogs) Debug.LogError("_First_Card is null!");
            return;
        }
        Show_Info(_first_card.Get_Sprite(), _first_card.Get_Description(), _first_card.Get_Name());
    }

    // Show card info on the side panel
    public void Show_Info(Sprite sprite, string info, string title)
    {
        imageSlot.sprite = sprite;
        InfoText.text = info;
        TitleText.text = title;
        imageSlot.gameObject.SetActive(true);
    }

    // Returns true if the player has clicked on the first card and a match hasnt been made yet
    public bool First_Card_selected()
    {
        if (_first_card == null)
            return false;
        else
            return true;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Default Screen");
    }

    private void Save_Progress()
    {
        if (_levels_won[0])
        {
            Debug.Log("Level3: Completed Game " + 1);
            PlayerPrefs.SetInt("Level3_1-Completed", 1);       // first game won
            PlayerPrefs.Save();
        }
        if (_levels_won[1])
        {
            Debug.Log("Level3: Completed Game " + 2);
            PlayerPrefs.SetInt("Level3_2-Completed", 1);       // second game won
            PlayerPrefs.Save();
        }
    }

    // lowers alpha if not yet completed
    // if game completed alpha = 1
    private void Highlight_Levels()
    {
        Color c = Color.white;
        int pref = PlayerPrefs.GetInt("Level3_1-Completed", 0);
        if (pref == 0)
        {
            c.a = 0.4f;
            level_button_image[0].color = c;
        }
        else if (pref == 1)
        {
            c.a = 1f;
            level_button_image[0].color = c;
            _levels_won[0] = true;
        }
        pref = PlayerPrefs.GetInt("Level3_2-Completed", 0);
        if (pref == 0)
        {
            c.a = 0.4f;
            level_button_image[1].color = c;
        }
        else if (pref == 1)
        {
            c.a = 1f;
            level_button_image[1].color = c;
            _levels_won[1] = true;
        }
    }
}