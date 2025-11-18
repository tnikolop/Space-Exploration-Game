using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Constellation", menuName = "Level 2/Constellation Data")]
public class Constellation_Data : ScriptableObject
{
    [Header("Πληροφορίες")]
    public string name;
    [TextArea] public string description;
    [TextArea] public string myth;

    [Header("Συντεταγμένες Αστεριών (0.1-0.9)")]    // pososto othonis
    public List<Vector2> star_positions;

    [Header("Συνδέσεις (Connections Index)")]    // pio asteri enonete me pio
    public List<Vector2Int> connections_index;
    

}
