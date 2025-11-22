using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float timeToWait = 1.0f; // Time before wrong pair closes
    [SerializeField] private bool showDebugLogs = true;


    private Memory_Card _first_card;
    private Memory_Card _second_card;
    private bool _is_checking = false;
    private bool _is_waiting_to_reset = false;      // wait for the clock to finish (cant open new card)
    private float _timer = 0f;                      // the clock
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup_Grid_Layout();
        Generate_Grid();
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
            card.name = $"Card_{id}";
            script.Setup(id, this);
        }
    }

    // Cant open a card if we are checking or we are waiting for the clock
    public bool Can_Flip()
    {
        return (!_is_checking && !_is_waiting_to_reset);
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
        _is_checking = true;

        if (_first_card.get_ID() == _second_card.get_ID())  // match
        {
            if (showDebugLogs) Debug.Log($"Match found for card id:{_first_card.get_ID()}");
            _first_card = null;
            _second_card = null;
            _is_checking = false;
            Global_Audio_Manager.Instance.Play_SFX_correct();
        }
        else    // Mismatch
        {
            if (showDebugLogs) Debug.Log("Mismatch");
            _timer = timeToWait;
            _is_waiting_to_reset = true;
            Global_Audio_Manager.Instance.Play_Error_SFX();
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
        _is_checking = false;
    }
}
