using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Solar/PlanetData")]
public class PlanetData : ScriptableObject
{
    public string planetName;
    public Sprite sprite;
    [TextArea(3,6)] public string shortDescription;
    [TextArea(1,3)] public string funFact;
    public int orderFromSun;
}
