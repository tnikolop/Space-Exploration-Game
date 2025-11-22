using UnityEngine;
using UnityEngine.UI;

public class Memory_Card : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    private int _id;    // the matching pair id
    private Level3_manager _manager;
    private bool _is_FaceUp;

    public void Setup(int id, Level3_manager manager)
    {
        _id = id;
        _manager = manager;
        image.color = Color.gray; //face down
    }

    public void Card_Clicked()
    {

    }

    public void Flip_Open()
    {

    }

    public void Flip_Closed()
    {

    }
    
    public void get_ID()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
