using UnityEngine;
using TMPro;

public class InfoDisplay_TMP : MonoBehaviour
{
    public static InfoDisplay_TMP Instance;

    [Header("UI references")]
    public TMP_Text titleText;      // Planet Name
    public TMP_Text descriptionText;
    public TMP_Text funFactText;
    public UnityEngine.UI.Image planetImage;       // for the sprite

    void Awake()
    {
        // Singleton pattern so  any script can call Infodisplay_TMP.Instance.ShowInfo()
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Public method to get the texts from PlanetData
    public void ShowInfo(PlanetData data)
    {
        UnityEngine.Debug.Log($"InfoDisplay_TMP.ShowInfo called for: {data?.planetName}");
        if (data == null)
        {
            HideInfo();
        }
        else
        {
            titleText.text = data.planetName;
            descriptionText.text = data.shortDescription;
            funFactText.text = data.funFact;
            if (planetImage != null && data.sprite != null)
            {
                planetImage.sprite = data.sprite;
                // make it visible again
                Color c = planetImage.color;
                c.a = 1f;
                planetImage.color = c;
            }
        }
    }

    public void HideInfo()
    {
        UnityEngine.Debug.Log("InfoDisplay_TMP.HideInfo called");
        // clear UI
        titleText.text = "";
        descriptionText.text = "";
        funFactText.text = "";
        if (planetImage != null)
            planetImage.sprite = null;

        // make the image transaprent so we dont have a white box left over when there is no planet selected
        Color c = planetImage.color;
        c.a = 0f;
        planetImage.color = c;

        // den ginetai planetImage.color.a = 0f gt to color einai struct kai den epireazei to original
    }
}
