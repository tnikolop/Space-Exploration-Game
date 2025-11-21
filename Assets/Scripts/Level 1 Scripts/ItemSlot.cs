using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public int expectedOrder;
    private bool occupied = false;

    // when a mouse click is released on the slot
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"OnDrop called on slot {expectedOrder}");
        if (eventData.pointerDrag == null)
        {
            Debug.Log("No pointerDrag detected.");
            return;
        }

        if (occupied)
        {
            Debug.Log("Slot already occupied!");
            Audio_Manager.Instance.Play_Error_SFX();
            return;
        }

        Drag_n_drop dragged = eventData.pointerDrag.GetComponent<Drag_n_drop>();
        if (dragged == null)
        {
            Debug.Log("Dragged object has no PlanetDrag component.");
            return;
        }

        if (dragged.data == null)
        {
            Debug.Log($"Dragged planet {dragged.name} has NULL PlanetData!");
            return;
        }
        Debug.Log($"Dragged planet: {dragged.data.planetName}, orderFromSun: {dragged.data.orderFromSun}");

        // check if correct order
        if (dragged.data.orderFromSun != expectedOrder)
        {
            Debug.Log($"Wrong slot! Planet {dragged.data.planetName} cannot go in slot {expectedOrder}");
            Audio_Manager.Instance.Play_Error_SFX();
            return;
        }

        // Snap planet to slot
        dragged.transform.SetParent(transform, false);
        dragged.transform.localPosition = Vector3.zero;
        Debug.Log($"Planet {dragged.data.planetName} placed correctly in slot {expectedOrder}");

        // Mark slot as occupied
        occupied = true;

        // tell GameManager this planet was placed correctly
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlanetLocked(expectedOrder);
        }
        else
            Debug.LogError("Game Manager Instance is null!!");

        // Play SFX for the correct planet placement
        if (Audio_Manager.Instance != null && Audio_Manager.Instance.correct_placement_sfx != null)
        {
            Audio_Manager.Instance.PlayCorrectPlacement_SFX();
        }
        else
            Debug.LogError("Audio Manager or correct_placement SFX is null!!");

    }
}
