using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class Level2_Manager : MonoBehaviour
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
    private Camera mainCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
        Load_Level();
    }

    // Clear previous Constellation and spawn new stars
    private void Load_Level()
    {
        // Clear last constellation if it exists
        foreach (Transform child in transform)  // game manager children
            Destroy(child.gameObject);
        spawned_stars.Clear();
        completed_connections.Clear();
        win_panel.SetActive(false);
        title_text.text = current_level_data.name;

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
                starting_star = star;
                drag_line.gameObject.SetActive(true);
                drag_line.SetPosition(0, starting_star.transform.position);
                drag_line.SetPosition(1, mouse_pos);
                starting_star.Set_Highlight(true);
            }
        }

        // Drag - update the line
        if(Input.GetMouseButtonDown(0) && starting_star != null)
        {
            drag_line.SetPosition(1, mouse_pos);
        }

        // Mouse Up - Check if there is a connection
        if (Input.GetMouseButtonDown(0) && starting_star != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouse_pos, Vector2.zero);
            bool connectionMade = false;

            if (hit.collider != null && hit.collider.TryGetComponent(out Star_point end_Star))
            {
                if (end_Star != starting_star)
                {
                    //check if connection is valid
                    if (Is_Valid_Connection(starting_star.id, end_Star.id) && !Connection_Exists(starting_star.id, end_Star.id))
                    {
                        Draw_Line(starting_star.transform.position, end_Star.transform.position);
                        Register_Connection(starting_star.id, end_Star.id);
                        connectionMade = true;
                        Check_Win_Condition();
                    }
                }
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
        if (completed_connections.Count >= current_level_data.connections_index.Count)
        {
            Debug.Log("Constellation Complete");
            win_panel.SetActive(true);
            win_description.text = current_level_data.description;
            win_myth.text = current_level_data.myth;
            // audio here
        }
    }
}
