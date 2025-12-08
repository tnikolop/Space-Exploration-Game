using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Memory_Card : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite back_sprite;
    [SerializeField] private Button button;
    [SerializeField] private bool showDebugLogs = true;

    private Card_data _data;
    private int _id;                                    // the matching pair id
    private Level3_manager _manager;
    private bool _is_FaceUp;

    public void Setup(int id, Level3_manager manager, Card_data data)
    {
        _id = id;
        _manager = manager;
        _data = data;
        Flip_Closed();
    }

    public void Card_Clicked()
    {
        if (showDebugLogs) Debug.Log($"CLicked on card id:{get_ID()}");
        if (_is_FaceUp)
        {    
            if (!_manager.First_Card_selected())       //if already open just show info
                _manager.Show_Info(Get_Sprite(),Get_Description(),Get_Name());
            return;
        }
        if (_manager.Can_Flip() == false)   // if 2 cards are open already
            return;

        Flip_Open();
        _manager.Card_Revealed(this);
    }

    public void Flip_Open()
    {
        _is_FaceUp = true;
        image.sprite = _data.sprite;
        image.color = Color.white;
        if (showDebugLogs) Debug.Log($"Flip_Open() called on card id:{get_ID()}");
    }

    public void Flip_Closed()
    {
        _is_FaceUp = false;
        image.sprite = back_sprite;
        image.color = Color.gray;
        if (showDebugLogs) Debug.Log($"Flip_Closed() called on card id:{get_ID()}");
    }

    public int get_ID()
    {
        return _id;
    }

    public string Get_Name()
    {
        return _data.name;
    }

    public string Get_Description()
    {
        return _data.description;
    }

    public Sprite Get_Sprite()
    {
        return _data.sprite;
    }
}
