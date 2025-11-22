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

    private Memory_Card _first_card;
    private Memory_Card _second_card;
    private bool _is_checking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup_Grid_Layout();
        Generate_Grid();
    }

    // function for dynamically setting up the grid layout
    private void Setup_Grid_Layout()
    {
        GridLayoutGroup grid_layout = grid_panel.GetComponent<GridLayoutGroup>();
        if (grid_layout == null)
            Debug.LogError("Grid Layout Component in Grid Panel does not exists!");

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

    private void Generate_Grid()
    {
        // Clean old cards
        foreach (Transform child in grid_panel)
            Destroy(child.gameObject);

        int total_cards = rows * cols;
        if (total_cards % 2 != 0)
        {
            Debug.LogError("Total Cards must be even!");
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
}
