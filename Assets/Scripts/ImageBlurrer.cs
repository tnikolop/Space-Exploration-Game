using UnityEngine;
using UnityEngine.UI;

public class ImageBlurrer : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite blurred_sprite;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = sprite;
    }

    public void ToggleBlur(bool toggle)
    {
        if (toggle)
            _image.sprite = blurred_sprite;
        else
            _image.sprite = sprite;
    }
}
