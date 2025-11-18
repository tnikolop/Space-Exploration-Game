using UnityEngine;

public class Star_point : MonoBehaviour
{
    public int id;  // number of this star on the constelletion vector
    private SpriteRenderer sprite_renderer;

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    // Changes slightly the color and the size of the star in order to highlight it
    public void Set_Highlight(bool active)
    {
        transform.localScale = active ? Vector3.one * 1.3f : Vector3.one;
        sprite_renderer.color = active ? Color.yellow : Color.white;
    }
}
