using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Drag_n_drop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public PlanetData data;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 _original_scale;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        _original_scale = transform.localScale;

        Debug.Log($"Drag_n_drop Awake: {data?.planetName}");
    }

    // When you click and drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag: {data.planetName}");
        startParent = transform.parent;

        // while dragging, allow raycasts to pass through this object so slots receive OnDrop
        canvasGroup.blocksRaycasts = false;

        // show info about the planet when draggin
        if (data != null)
        {
            if (InfoDisplay_TMP.Instance != null)
                InfoDisplay_TMP.Instance.ShowInfo(data);
            else
                Debug.Log("No InfoDisplay instance found in scene.");
        }
        else
            Debug.LogWarning($"{name} has no PlanetData assigned!");

        // Move to top-level Canvas to avoid being clipped by layout groups
        transform.SetParent(canvas.transform, true);
    }

    // While you drag
    public void OnDrag(PointerEventData eventData)
    {
        // Move the UI element by pointer delta, adjusted for canvas scale
        // in our case the scale is 1 so its obsolete
        transform.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    // when you stop draggin the planet
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag: {data.planetName}");
        canvasGroup.blocksRaycasts = true;

        // If dropped somewhere invalid, return to start
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(startParent, false);
            transform.localPosition = Vector3.zero;
            Debug.Log($"{data.planetName} returned to start");
        }
    }

    // When a planet is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Drag_n_drop.OnPointerClick called for {data?.planetName}");
        // show info about the planet when clicked
        if (data != null)
        {
            if (InfoDisplay_TMP.Instance != null)
                InfoDisplay_TMP.Instance.ShowInfo(data);
            else
                Debug.Log("No InfoDisplay instance found in scene.");
        }
        else
            Debug.LogWarning($"{name} has no PlanetData assigned!");
    }

    // make the planet larger
    public void Highlight(bool active)
    {
        if (active)
        {
            transform.localScale = _original_scale * 1.3f;
        }
        else
        {
            transform.localScale = _original_scale;
        }
    }

    // Helper function for the hint logic
    // returns true if the planet is placed in the correct slot
    public bool Is_Correctly_Placed()
    {
        ItemSlot slot = GetComponentInParent<ItemSlot>();

        if (slot != null && slot.expectedOrder == data.orderFromSun)
            return true;
        else
            return false;
    }
}
