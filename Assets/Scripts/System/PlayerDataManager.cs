using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager _instance { get; private set; }
    [SerializeField] private GameObject _persistentUICanvas;
    [SerializeField] private List<string> _scenesToHideUI = new List<string> { "MainMenu", "ShopScene" };
    [SerializeField] private int _defaultMaxHP = 100;
    [SerializeField] private int _potionAmount = 25;
    private int _maxHP;
    private int _collectedCoins;
    private int _totalPotions;
    private bool _hasPistol;
    private int _currentHealth;
    private HashSet<string> _collectedItemIDs;
    private const string _HEALTH_KEY = "GlobalMaxHealth";
    private const string _PISTOL_KEY = "GlobalHasPistol";
    private const string _COINS_KEY = "GlobalCoins";
    private const string _POTIONS_KEY = "GlobalPotions";
    public event Action _OnHealthChanged;
    public event Action _OnCoinsChanged;
    public event Action _OnInventoryChanged;
    public event Action _OnGameSaved;
    private static bool _isLoadingGame = false;
    private static Vector3 _positionToLoad;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadGlobalStats();
            this._currentHealth = this._maxHP;
            this._collectedItemIDs = new HashSet<string>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    
    private void LoadGlobalStats()
    {
        _maxHP = PlayerPrefs.GetInt(_HEALTH_KEY, _defaultMaxHP);
        _collectedCoins = PlayerPrefs.GetInt(_COINS_KEY, 0); 
        _hasPistol = PlayerPrefs.GetInt(_PISTOL_KEY, 0) == 1;
        _totalPotions = PlayerPrefs.GetInt(_POTIONS_KEY, 0);
    }

    
    private void SaveGlobalStats()
    {
        PlayerPrefs.SetInt(_HEALTH_KEY, _maxHP);
        PlayerPrefs.SetInt(_COINS_KEY, _collectedCoins);
        PlayerPrefs.SetInt(_PISTOL_KEY, _hasPistol ? 1 : 0);
        PlayerPrefs.SetInt(_POTIONS_KEY, _totalPotions);
        PlayerPrefs.Save();
    }


    public void StartNewGame()
    {
        GameData data = new GameData(this._maxHP); 
        ApplySessionData(data);

        SaveSystem.SaveGame(data);

        this._currentHealth = this._maxHP;
        _OnHealthChanged?.Invoke();
        _OnCoinsChanged?.Invoke(); 

        _isLoadingGame = false;
        data.currentSceneName = "Cutscene_Level1";
        SceneManager.LoadScene(data.currentSceneName);
    }

    public void LoadGame()
    {
        LoadGlobalStats();

        GameData data = SaveSystem.LoadGame();
        if (data == null)
        {
            StartNewGame();
            return;
        }

        ApplySessionData(data);
        _isLoadingGame = true;
        _positionToLoad = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        SceneManager.LoadScene(data.currentSceneName);
    }

    public void SaveGame(Vector3 playerPosition)
    {
        SaveGlobalStats();

        GameData data = new GameData(this._maxHP);
        data.currentHealth = this._currentHealth;
        data.collectedItemIDs = this._collectedItemIDs;
        data.currentSceneName = SceneManager.GetActiveScene().name;
        data.playerPosition = new float[3] { playerPosition.x, playerPosition.y, playerPosition.z };

        SaveSystem.SaveGame(data);
        _OnGameSaved?.Invoke();
    }

    private void ApplySessionData(GameData data)
    {
        this._currentHealth = data.currentHealth;
        this._collectedItemIDs = data.collectedItemIDs;

        _OnHealthChanged?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCanvasVisibility(scene);
        if (_isLoadingGame)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) player.transform.position = _positionToLoad;
            _isLoadingGame = false;
        }
    }
    private void CheckCanvasVisibility(Scene scene)
    {
        if (_persistentUICanvas == null) return;
        _persistentUICanvas.SetActive(!_scenesToHideUI.Contains(scene.name));
    }
    private void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHP;
    public int GetCoinCount() => _collectedCoins;
    public int GetTotalPotions() => _totalPotions;
    public bool GetHasPistol() => _hasPistol;
    public bool IsItemCollected(string id)
    {
        if (string.IsNullOrEmpty(id) || _collectedItemIDs == null) return false;
        return _collectedItemIDs.Contains(id);
    }


    public void ReportItemCollected(string id)
    {
        if (_collectedItemIDs.Contains(id)) return;
        _collectedItemIDs.Add(id);
    }
    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth < 0) _currentHealth = 0;
        _OnHealthChanged?.Invoke();
    }
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > _maxHP) _currentHealth = _maxHP;
        _OnHealthChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        _collectedCoins += amount;
        _OnCoinsChanged?.Invoke();
        SaveGlobalStats(); 
    }
    public bool SpendCoins(int amountToSpend)
    {
        if (_collectedCoins >= amountToSpend)
        {
            _collectedCoins -= amountToSpend;
            _OnCoinsChanged?.Invoke();
            SaveGlobalStats();
            return true;
        }
        return false;
    }
    public void AddPotion()
    {
        _totalPotions++;
        _OnInventoryChanged?.Invoke();
        SaveGlobalStats();
    }
    public void GrantPistol()
    {
        if (_hasPistol) return;
        _hasPistol = true;
        _OnInventoryChanged?.Invoke();
        SaveGlobalStats();
    }
    public void IncreaseMaxHealth(int amount)
    {
        _maxHP += amount;
        Heal(amount);
        SaveGlobalStats();
    }

    public bool AddHealth(float amount)
    {
        if (_currentHealth >= _maxHP)
        {
            return false; 
        }

        _currentHealth += (int)amount;

        if (_currentHealth > _maxHP)
        {
            _currentHealth = _maxHP;
        }

        _OnHealthChanged?.Invoke();
        return true; 
    }

    public void UsePotion()
    {
        if (_totalPotions <= 0) return;
        if (_currentHealth >= _maxHP) return;

        _totalPotions--; 
        Heal(_potionAmount); 

        _OnInventoryChanged?.Invoke();
        SaveGlobalStats(); 
    }
}