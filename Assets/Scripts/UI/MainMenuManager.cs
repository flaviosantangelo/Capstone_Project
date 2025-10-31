using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnNewGameButtonPress()
    {
        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance.StartNewGame();
        }
    }

    public void OnLoadGameButtonPress()
    {
        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance.LoadGame();
        }
    }

    public void OnShopButtonPress()
    {
        SceneManager.LoadScene("ShopScene"); 
    }

    public void OnQuitButtonPress()
    {
        Application.Quit();
    }
}