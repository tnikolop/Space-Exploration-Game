using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class Level2_Manager : MonoBehaviour
{

    [Header("Debugging")]
    [SerializeField] private bool showDebugLogs = true;

    [Header("Settings")]
    [SerializeField] private List<Constellation_Data> constellation_data_list;
    [SerializeField] private GameObject star_prefab;
    [SerializeField] private GameObject line_prefab;
    [SerializeField] private LineRenderer drag_line;    // the line the player will be creating when draggin

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI title_text;
    [SerializeField] private GameObject win_panel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI myth;
    [SerializeField] private GameObject next_level_button;

    private List<Star_point> spawned_stars = new List<Star_point>();    // list with all the current spawned stars
    private HashSet<string> completed_connections = new HashSet<string>();  // stores all the completed lines/ star connections
    private Star_point starting_star;   // from which star the player started drawing
    private Camera mainCam;

    private Constellation_Data current_level_data;  // ta data tou torinou asterismou
    private int current_level_index; // se pio shmeio ths listas eimaste

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
        if (drag_line == null) Debug.LogError("drag_line is null!");
        if (constellation_data_list.Count == 0) Debug.LogError("no constellation data found!");
        current_level_index = 0;
        Load_Level();
    }

    public void Load_next_level()
    {
        if (current_level_index < constellation_data_list.Count)
        {
            current_level_index++;
            Load_Level();
        }
        else    // finshed minigame
        {
            // go to main menu
        }
    }

    // Clear previous Constellation and spawn new stars
    private void Load_Level()
    {
        // load the data
        current_level_data = constellation_data_list[current_level_index];

        // Clear last constellation if it exists
        foreach (Transform child in transform)  // game manager children
        {
            if (child == drag_line.transform)   // dont destroy the drag line 
                continue;   
            Destroy(child.gameObject);
        }
        spawned_stars.Clear();
        completed_connections.Clear();
        win_panel.SetActive(false);

        if (current_level_data != null)
        {
            title_text.text = current_level_data.name;
            description.text = current_level_data.description;
            if(showDebugLogs) Debug.Log($"Loaded: {current_level_data.name} with {current_level_data.star_positions.Count} stars.");
        }

        // Spawn the stars
        for (int i = 0; i < current_level_data.star_positions.Count; i++)
        {
            Vector2 normalized_pos = current_level_data.star_positions[i];
            Vector3 world_pos = Get_World_Position(normalized_pos);

            // create the new stars from the prefab
            GameObject new_Star = Instantiate(star_prefab, world_pos, Quaternion.identity, transform);
            Star_point star_script = new_Star.GetComponent<Star_point>();
            star_script.id = i;     // assign id to the star
            spawned_stars.Add(star_script);
        }
    }


    // Update is called once per frame
    void Update()
    {
        Handle_Input();
    
    }
    private void Handle_Input()
    {
        // mouse pos from pixels to world coords
        Vector3 mouse_pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouse_pos.z = 0;

        // First Click - Mouse down - We start drawind the line
        if (Input.GetMouseButtonDown(0))
        {
            // cast ray to see if we hit an object
            RaycastHit2D hit = Physics2D.Raycast(mouse_pos, Vector2.zero);
            // check if the object the ray hit(mouse click) was a star
            if (hit.collider != null && hit.collider.TryGetComponent(out Star_point star))
            {
                if (showDebugLogs) Debug.Log($"Star_ID: {star.id} was clicked");
                starting_star = star;
                drag_line.gameObject.SetActive(true);
                drag_line.SetPosition(0, starting_star.transform.position);
                drag_line.SetPosition(1, mouse_pos);
                starting_star.Set_Highlight(true);
            }
        }

        // Drag - update the line
        if(Input.GetMouseButton(0) && starting_star != null)
        {
            drag_line.SetPosition(1, mouse_pos);
        }

        // Mouse Up - Check if there is a connection
        if (Input.GetMouseButtonUp(0) && starting_star != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouse_pos, Vector2.zero);
            // bool connectionMade = false;

            if (hit.collider != null && hit.collider.TryGetComponent(out Star_point end_Star))
            {
                if (showDebugLogs) Debug.Log($"2. You stopped draggin at star_id: {end_Star.id}");
                if (end_Star != starting_star)
                {
                    bool valid = Is_Valid_Connection(starting_star.id, end_Star.id);
                    bool exists = Connection_Exists(starting_star.id, end_Star.id);
                    //check if connection is valid
                    if (valid && !exists)
                    {
                        if (showDebugLogs) Debug.Log("Connection Succesful!");
                        Draw_Line(starting_star.transform.position, end_Star.transform.position);
                        Register_Connection(starting_star.id, end_Star.id);
                        // connectionMade = true;
                        Check_Win_Condition();
                    }
                    else
                    {
                        if (showDebugLogs) Debug.Log($"Conection Failed! : Valid={valid}, Exists={exists}");
                    }
                }
                else
                {
                    if (showDebugLogs) Debug.Log("Conection Failed! You clicked on the same star");
                }
            }
            else
            {
                if(showDebugLogs) Debug.Log("Mouse up on empty space.");
            }

            //reset
            starting_star.Set_Highlight(false);
            starting_star = null;
            drag_line.gameObject.SetActive(false);
        }
    }

    // transforms the star positions (0.2,0.3) to Unity coords 
    // this way it will work on all screens and sizes
    private Vector3 Get_World_Position(Vector2 normalized_pos)
    {
        float height = 2f * mainCam.orthographicSize;
        float width = height * mainCam.aspect;

        // calculate new pos based on the center(0,0)
        float x = (normalized_pos.x - 0.5f) * width;
        float y = (normalized_pos.y - 0.5f) * height;

        return new Vector3(x, y, 0);
    }

    // check if the connection bettween 2 stars is valid
    private bool Is_Valid_Connection(int ida, int idb)
    {
        foreach (var pair in current_level_data.connections_index)
        {
            if ((pair.x == ida && pair.y == idb) || (pair.x == idb && pair.y == ida))
                return true;
        }
        return false;
    }

    // check if connection exists
    private bool Connection_Exists(int ida, int idb)
    {
        string key1 = ida + "-" + idb;
        string key2 = idb + "-" + ida;
        return completed_connections.Contains(key1) || completed_connections.Contains(key2);

    }

    // register new connection
    private void Register_Connection(int ida, int idb)
    {
        completed_connections.Add(ida + "-" + idb);
        if(showDebugLogs) Debug.Log($"Connection Registered {ida}-{ida}. Total: {completed_connections.Count}");
    }

    // Draw a permanent line bettween 2 start to mark a succesful connection
    // input: star coordinates
    private void Draw_Line(Vector3 start, Vector3 end)
    {
        // create a clone of the line prefab pou exoume eidh kanei sto unity editor
        GameObject lineObj = Instantiate(line_prefab, Vector3.zero, Quaternion.identity, transform);    // balto sto (0,0,0) kai paidi toy GameManager

        LineRenderer line_renderer = lineObj.GetComponent<LineRenderer>();
        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
    }

    // Check if the Win Condition is met
    // Win Condition: current connection >= expected connections for this constellation
    private void Check_Win_Condition()
    {
        if(showDebugLogs) Debug.Log($"Check Win: Completed Connections:{completed_connections.Count} / {current_level_data.connections_index.Count}.");

        if (completed_connections.Count >= current_level_data.connections_index.Count)
        {
            Debug.Log("Constellation Complete");
            win_panel.SetActive(true);
            myth.text = current_level_data.myth;
            // audio here

            if (current_level_index >= constellation_data_list.Count - 1)
            {
                if (next_level_button != null)
                {
                    next_level_button.SetActive(false);
                    // end message
                }
            }
            else
            {
                if (next_level_button != null)
                {
                    next_level_button.SetActive(true);
                }
            }

        }
    }
}
