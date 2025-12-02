using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Drag_n_drop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public PlanetData data;

    private Vector3 startPos;
    private Transform startParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Image _image;
    private Color _original_color;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        _image = GetComponent<Image>();
        if (_image != null)
        {
            _original_color = _image.color;
        }

        Debug.Log($"Drag_n_drop Awake: {data?.planetName}");
    }

    // When you click and drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag: {data.planetName}");
        startPos = transform.position;
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

    public void Highlight(bool active)
    {
        if (_image == null)
        {
            Debug.LogError("Planet IMage is null");
            return;
        }
        if (active)
        {
            _image.color = Color.yellow;
        }
        else
        {
            _image.color = _original_color;
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
