using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_menu_buttons : MonoBehaviour
{
    public void OpenLevel(int level_id)
    {
        SceneManager.LoadScene("Level " + level_id);
    }
}
