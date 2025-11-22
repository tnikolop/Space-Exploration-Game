using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Memory_Card : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private int _id;    // the matching pair id
    private Level3_manager _manager;
    private bool _is_FaceUp;

    public void Setup(int id, Level3_manager manager)
    {
        _id = id;
        _manager = manager;
        text.text = _id.ToString();
        Flip_Closed();
    }

    public void Card_Clicked()
    {
        Debug.Log($"CLicked on card id:{get_ID()}");
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
        image.color = Color.green;
        text.gameObject.SetActive(true);
        Debug.Log($"Flip_Open() called on card id:{get_ID()}");
    }

    public void Flip_Closed()
    {
        _is_FaceUp = false;
        image.color = Color.gray;
        text.gameObject.SetActive(false);
        Debug.Log($"Flip_Closed() called on card id:{get_ID()}");
    }
    
    public int get_ID()
    {
        return _id;
    }
}
