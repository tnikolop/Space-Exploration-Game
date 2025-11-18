using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class Game_Manager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Constellation_Data current_level_data;
    [SerializeField] private GameObject star_prefab;
    [SerializeField] private GameObject line_prefab;
    [SerializeField] private LineRenderer drag_line;    // the line the player will be creating when draggin

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI title_text;
    [SerializeField] private GameObject win_panel;
    [SerializeField] private TextMeshProUGUI win_description;
    [SerializeField] private TextMeshProUGUI win_myth;

    private List<Star_point> spawned_stars = new List<Star_point>();    // list with all the current spawned stars
    private HashSet<string> completed_connections = new HashSet<string>();  // stores all the completed lines/ star connections
    private Star_point starting_star;   // from which star the player started drawing

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Load_Level();
    }

    private void Load_Level()
    {

    }

    private void Handle_Input()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
