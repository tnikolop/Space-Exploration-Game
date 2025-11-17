using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickCatcher : MonoBehaviour, IPointerClickHandler
{
    // it is called when the background is clicked so the planet info disappears
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Background clicked — hiding info");
        if (InfoDisplay_TMP.Instance != null)
            InfoDisplay_TMP.Instance.HideInfo();
    }
}
