using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections; 

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _saveBannerText;
    [SerializeField] private float _bannerDuration = 2.0f;
    [SerializeField] private TextMeshProUGUI _alertText; 
    [SerializeField] private float _alertDisplayTime = 3.0f; 
    private Coroutine _alertCoroutine; 
    private GameManager _currentSceneGameManager;

    void Start()
    {
        if (PlayerDataManager._instance == null)
        {
            return;
        }

        PlayerDataManager._instance._OnHealthChanged += UpdateHealthUI;
        PlayerDataManager._instance._OnCoinsChanged += UpdateCoinUI;
        EnemyAlertManager._onPlayerSpotted += HandlePlayerSpotted;
        EnemyAlertManager._onEnemySearchFinished += HandleSearchFinished;

        if (_alertText != null) _alertText.gameObject.SetActive(false);

        PlayerDataManager._instance._OnGameSaved += HandleGameSaved;
        if (_saveBannerText != null) _saveBannerText.gameObject.SetActive(false);

        InitializeUI();
    }

    private void InitializeUI()
    {
        UpdateHealthUI();
        UpdateCoinUI();
    }
    private void UpdateHealthUI()
    {
        if (_healthFillImage == null) return;
        float fillRatio = (float)PlayerDataManager._instance.GetCurrentHealth() / (float)PlayerDataManager._instance.GetMaxHealth();
        _healthFillImage.fillAmount = fillRatio;
    }
    private void UpdateCoinUI()
    {
        if (_coinText == null) return;
        _coinText.text = PlayerDataManager._instance.GetCoinCount().ToString();
    }
    private void OnDestroy()
    {
        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance._OnHealthChanged -= UpdateHealthUI;
            PlayerDataManager._instance._OnCoinsChanged -= UpdateCoinUI;
            EnemyAlertManager._onPlayerSpotted -= HandlePlayerSpotted;
            EnemyAlertManager._onEnemySearchFinished -= HandleSearchFinished;
        }

        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance._OnGameSaved -= HandleGameSaved;
        }
    }
    private GameManager GetCurrentGameManager()
    {
        if (_currentSceneGameManager == null)
        {
            _currentSceneGameManager = FindObjectOfType<GameManager>();

            if (_currentSceneGameManager == null)
            { }
        }
        return _currentSceneGameManager;
    }

    public void OnResumeButtonPress()
    {
        GetCurrentGameManager()?.TogglePause();
    }

    public void OnRestartButtonPress()
    {
        GetCurrentGameManager()?.RestartLevel();
    }

    public void OnMainMenuButtonPress()
    {
        GetCurrentGameManager()?.LoadMainMenu();
    }

    public void OnNextLevelButtonPress()
    {
        GetCurrentGameManager()?.LoadNextLevel();
    }
    private void HandlePlayerSpotted()
    {
        ShowAlert("Attenzione, sei stato individuato!");
    }
    private void HandleSearchFinished()
    {
        ShowAlert("Il nemico ha smesso di cercarti");
    }
    private void ShowAlert(string message)
    {
        if (_alertText == null) return;

        if (_alertCoroutine != null)
        {
            StopCoroutine(_alertCoroutine);
        }

        _alertText.text = message;
        _alertText.gameObject.SetActive(true);
        _alertCoroutine = StartCoroutine(HideAlertAfterTime());
    }

    
    private IEnumerator HideAlertAfterTime()
    {
        yield return new WaitForSeconds(_alertDisplayTime);
        _alertText.gameObject.SetActive(false);
        _alertCoroutine = null;
    }
    private void HandleGameSaved()
    {
        if (_saveBannerText != null)
        {
            StartCoroutine(ShowSaveBanner());
        }
    }
    private IEnumerator ShowSaveBanner()
    {
        _saveBannerText.text = "Salvataggio in corso...";
        _saveBannerText.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < _bannerDuration)
        { 
            _saveBannerText.enabled = !_saveBannerText.enabled;
            timer += 0.2f;
            yield return new WaitForSecondsRealtime(0.2f); 
        }

        _saveBannerText.gameObject.SetActive(false);
        _saveBannerText.enabled = true; 
    }
}