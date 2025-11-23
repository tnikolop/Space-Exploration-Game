using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Level3_manager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private Transform grid_panel; // The Panel holding the cards
    [SerializeField] private GameObject card_prefab;   // The card object to spawn
    [SerializeField] private int rows = 4;
    [SerializeField] private int cols = 3;
    [SerializeField] private float padding = 20f;     // Space between cards
    [SerializeField] private int edge_padding = 20;

    [Header("Game Settings")]
    [SerializeField] private float timeToWait = 1.0f; // Time before wrong pair closes in seconds
    [SerializeField] private bool showDebugLogs = false;

    [SerializeField] private Transform GameInfoPanel;
    [SerializeField] private Transform InfoPanel;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private Image imageSlot;
    [Header("Levels Buttons")]
    [SerializeField] private Image[] level_button_image;



    [Header("Data lists")]
    [SerializeField] private List<Card_data> level1_data;
    [SerializeField] private List<Card_data> level2_data;
    [SerializeField] private List<Card_data> level3_data;


    private List<Card_data> _current_level_data;
    private Memory_Card _first_card;                // first card flipped open
    private Memory_Card _second_card;               // second card flipped open
    private bool _is_waiting_to_reset = false;      // wait for the clock to finish (cant open new card)
    private float _timer = 0f;                      // the clock
    private bool[] _levels_won = new bool[3];        // true if a level has been won (automatically initialized to false in C#)
    private int _current_level_index;               // current level playing
    private int match_count = 0;                    // how many matches have been found, (win condition check)



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInfoPanel.gameObject.SetActive(true);
        InfoPanel.gameObject.SetActive(false);
        grid_panel.gameObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {   
            Color temp = level_button_image[i].color;
            temp.a = 0.4f;
            level_button_image[i].color = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // clock
        if (_is_waiting_to_reset)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Close_Mismatch();
            }
        }
    }

    // Load necessary game data
    public void Select_Level(int level_number)
    {
        _current_level_index = level_number;
        switch (level_number)
        {
            case 0:
                _current_level_data = level1_data;
                break;
            case 1:
                _current_level_data = level2_data;
                break;
            case 2:
                _current_level_data = level3_data;
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
            imageSlot.sprite = _first_card.Get_Sprite();
            InfoText.text = _first_card.Get_Description();
            TitleText.text = _first_card.Get_Name();
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
            // imageSlot.sprite = null;
            // InfoText.text = null;
            // TitleText.text = null;
        }
    }

    private void Check_Win()
    {
        if (match_count >= _current_level_data.Count)
        {
            Global_Audio_Manager.Instance.Play_Win_SFX();
            _levels_won[_current_level_index] = true;
            Color temp = Color.white;
            temp.a = 1f;
            level_button_image[_current_level_index].color = temp;
            Check_Game_Completed();
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
        if (_levels_won[0] && _levels_won[1] && _levels_won[2])
        {
            // GAME COMPLETE
        }
    }
}
