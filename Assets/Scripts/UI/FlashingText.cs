using UnityEngine;

public class FlashingText : MonoBehaviour
{
    [SerializeField] private GameObject _promptText;
    [SerializeField] private GameObject _initialFlashingText;
    [SerializeField] private GameObject _pcMenuCanvas;
    private bool _isPlayerNearby = false;
    private bool _isMenuOn = false;

    void Start()
    {
        if (_promptText != null) _promptText.SetActive(false);
        if (_pcMenuCanvas != null) _pcMenuCanvas.SetActive(false);
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

        if (_pcMenuCanvas != null) _pcMenuCanvas.SetActive(true);

        if (_promptText != null) _promptText.SetActive(false);
        if (_initialFlashingText != null) _initialFlashingText.SetActive(false);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isMenuOn)
        {
            _isPlayerNearby = true;
            if (_promptText != null) _promptText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            if (_promptText != null) _promptText.SetActive(false);
        }
    }
}