using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Memory_Card : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite back_sprite;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool showDebugLogs = true;

    private Card_data data;


    private int _id;    // the matching pair id
    private Level3_manager _manager;
    private bool _is_FaceUp;
    private Sprite _face_sprite;
    public void Setup(int id, Level3_manager manager, Sprite face_sprite)
    {
        _id = id;
        _manager = manager;
        _face_sprite = face_sprite;
        text.text = _id.ToString();
        Flip_Closed();
    }

    public void Card_Clicked()
    {
        if (showDebugLogs) Debug.Log($"CLicked on card id:{get_ID()}");
        if (_is_FaceUp)     //if already open
            return;
        if (_manager.Can_Flip() == false)   // if 2 cards are open already
            return;

        Flip_Open();
        _manager.Card_Revealed(this);
    }

    public void Flip_Open()
    {
        _is_FaceUp = true;
        image.sprite = _face_sprite;
        // image.color = Color.green;
        text.gameObject.SetActive(true);
        if (showDebugLogs) Debug.Log($"Flip_Open() called on card id:{get_ID()}");
    }

    public void Flip_Closed()
    {
        _is_FaceUp = false;

        image.sprite = back_sprite;
        image.color = Color.gray;
        text.gameObject.SetActive(false);
        if (showDebugLogs) Debug.Log($"Flip_Closed() called on card id:{get_ID()}");
    }
    
    public int get_ID()
    {
        return _id;
    }
}
