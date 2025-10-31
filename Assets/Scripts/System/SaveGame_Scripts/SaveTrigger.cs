using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(KeyCode.S))
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }

            if (PlayerDataManager._instance != null)
            {
                PlayerDataManager._instance.SaveGame(transform.position);
            }
        }
    }
}