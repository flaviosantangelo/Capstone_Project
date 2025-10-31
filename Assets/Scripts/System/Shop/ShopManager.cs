using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int _potionCost = 10;
    [SerializeField] private int _healthUpgradeCost = 50;
    [SerializeField] private int _pistolCost = 200;
    [SerializeField] private int _healthUpgradeAmount = 10;

    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    [SerializeField] private TextMeshProUGUI _potionInventoryText;
    [SerializeField] private TextMeshProUGUI _healthLevelText; 
    [SerializeField] private Button _potionBuyButton;
    [SerializeField] private Button _healthBuyButton;
    [SerializeField] private Button _pistolBuyButton;

    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    void Start()
    {
        if (PlayerDataManager._instance == null)
        {
            return;
        }

        PlayerDataManager._instance._OnCoinsChanged += UpdateUI;
        PlayerDataManager._instance._OnInventoryChanged += UpdateUI;
        PlayerDataManager._instance._OnHealthChanged += UpdateUI; 

        UpdateUI();
    }

    void UpdateUI()
    {
        int currentCoins = PlayerDataManager._instance.GetCoinCount();

        _totalCoinsText.text = "Monete: " + currentCoins;
        _potionInventoryText.text = "Pozioni: " + PlayerDataManager._instance.GetTotalPotions();
        _potionBuyButton.interactable = (currentCoins >= _potionCost);
        if (PlayerDataManager._instance.GetHasPistol())
        {
            _pistolBuyButton.interactable = false;
        }
        else
        {
            _pistolBuyButton.interactable = (currentCoins >= _pistolCost);
        }
    }
    public void OnBuyPotionClicked()
    {
        if (PlayerDataManager._instance.SpendCoins(_potionCost))
        {
            PlayerDataManager._instance.AddPotion();
        }
    }

    public void OnBuyHealthUpgradeClicked()
    {
        if (PlayerDataManager._instance.SpendCoins(_healthUpgradeCost))
        {
            PlayerDataManager._instance.IncreaseMaxHealth(_healthUpgradeAmount);
        }
    }

    public void OnBuyPistolClicked()
    {
        if (PlayerDataManager._instance.GetHasPistol()) return; 

        if (PlayerDataManager._instance.SpendCoins(_pistolCost))
        {
            PlayerDataManager._instance.GrantPistol();
        }
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    private void OnDestroy()
    {
        if (PlayerDataManager._instance != null)
        {
            PlayerDataManager._instance._OnCoinsChanged -= UpdateUI;
            PlayerDataManager._instance._OnInventoryChanged -= UpdateUI;
            PlayerDataManager._instance._OnHealthChanged -= UpdateUI;
        }
    }
}