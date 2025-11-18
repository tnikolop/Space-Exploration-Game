using UnityEngine;

public class Star_point : MonoBehaviour
{
    public int id;  // number of this star on the constelletion vector
    private SpriteRenderer sprite_renderer;
    private Vector3 original_scale;

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        original_scale = transform.localScale;
    }

    // Changes slightly the color and the size of the star in order to highlight it
    public void Set_Highlight(bool active)
    {
        transform.localScale = active ? original_scale * 1.3f : original_scale;
        sprite_renderer.color = active ? Color.yellow : Color.white;
    }
}
