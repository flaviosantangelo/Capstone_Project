using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Collider2D))] 
public class PCTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _promptsGroupOverlay;
    [SerializeField] private GameObject _promptWorldCanvas;
    [SerializeField] private GameObject _mainMenuGroup;
    [SerializeField] private float _menuFadeDuration = 1.0f;
    private bool _isPlayerNearby = false;
    private bool _isMenuOn = false;
    private CanvasGroup _mainMenuGroupCanvasGroup; 

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (_mainMenuGroup != null)
        {
            _mainMenuGroupCanvasGroup = _mainMenuGroup.GetComponent<CanvasGroup>();
        }
      
        if (_mainMenuGroup != null) _mainMenuGroup.SetActive(false);
        if (_promptWorldCanvas != null) _promptWorldCanvas.SetActive(false);
        if (_promptsGroupOverlay != null) _promptsGroupOverlay.SetActive(true);
    }

    void Update()
    {
        if (_isPlayerNearby && Input.GetKeyDown(KeyCode.I) && !_isMenuOn)
        {
            ActivateMenu();
        }
    }
    private void ActivateMenu()
    {
        _isMenuOn = true;

        if (_promptsGroupOverlay != null) _promptsGroupOverlay.SetActive(false);
        if (_promptWorldCanvas != null) _promptWorldCanvas.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (_mainMenuGroup != null && _mainMenuGroupCanvasGroup != null)
        {
            _mainMenuGroup.SetActive(true);
            StartCoroutine(FadeInMenuRoutine());
        }
    }

    private IEnumerator FadeInMenuRoutine()
    {
        _mainMenuGroupCanvasGroup.alpha = 0f;
        _mainMenuGroupCanvasGroup.interactable = false;
        _mainMenuGroupCanvasGroup.blocksRaycasts = false;

        float timer = 0f;
        while (timer < _menuFadeDuration)
        {
            timer += Time.deltaTime;
            _mainMenuGroupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / _menuFadeDuration);
            yield return null;
        }

        _mainMenuGroupCanvasGroup.alpha = 1f;
        _mainMenuGroupCanvasGroup.interactable = true;
        _mainMenuGroupCanvasGroup.blocksRaycasts = true;
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !_isMenuOn)
        {
            _isPlayerNearby = true;
            if (_promptWorldCanvas != null) _promptWorldCanvas.SetActive(true);
            if (_promptsGroupOverlay != null) _promptsGroupOverlay.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !_isMenuOn)
        {
            _isPlayerNearby = false;
            if (_promptWorldCanvas != null) _promptWorldCanvas.SetActive(false);
            if (_promptsGroupOverlay != null) _promptsGroupOverlay.SetActive(true);
        }
    }
}