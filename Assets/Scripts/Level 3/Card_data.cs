using UnityEngine;

[CreateAssetMenu(menuName = "Level 3/Card Data")]
public class Card_data : ScriptableObject
{
    [Header("Πληροφορίες")]
    public new string name;
    public Sprite sprite;
    [TextArea] public string description;
}
