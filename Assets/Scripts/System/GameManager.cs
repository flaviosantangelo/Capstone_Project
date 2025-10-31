using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _levelCompleteUI;
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private string _shopSceneName = "ShopScene";
    [SerializeField] private string _nextLevelName;
    private bool _isPaused = false;
    private bool _isGameOver = false;

    void Start()
    {
        _isPaused = false;
        _isGameOver = false;
        Time.timeScale = 1f;

        if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        if (_gameOverUI != null) _gameOverUI.SetActive(false);
        if (_levelCompleteUI != null) _levelCompleteUI.SetActive(false);

        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance._OnHealthChanged += CheckForGameOver;
            CheckForGameOver();
        }
    }

    void Update()
    {
        if (_isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        if (_isGameOver) return;

        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        }
    }
    public void OnMainMenuButtonPress()
    {
        LoadMainMenu();
    }

    public void OnShopButtonPress()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_shopSceneName);
    }

    public void HandleLevelComplete()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        if (_levelCompleteUI != null)
        {
            _levelCompleteUI.SetActive(true);
        }
    }

    public void LoadOutroCutscene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level3_final");
    }
     


    public void LoadNextLevel()
    {
        if (string.IsNullOrEmpty(_nextLevelName))
        {
            LoadMainMenu();
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(_nextLevelName);
    }


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }


    private void CheckForGameOver()
    {
        if (PlayerDataManager._instance.GetCurrentHealth() <= 0 && !_isGameOver)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f;
        if (_gameOverUI != null)
        {
            _gameOverUI.SetActive(true);
        }
    }
    private void OnDestroy()
    {

        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance._OnHealthChanged -= CheckForGameOver;
        }
    }
}