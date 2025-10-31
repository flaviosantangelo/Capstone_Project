using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _obstaclesToDisable;
    [SerializeField] private GameObject _interactionPrompt; 
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    private bool _isPlayerNearby = false;
    private bool _hasBeenPressed = false;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (_interactionPrompt != null)
        {
            _interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (_isPlayerNearby && Input.GetKeyDown(_interactionKey) && !_hasBeenPressed)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        _hasBeenPressed = true;

        if (_obstaclesToDisable != null)
        {
            _obstaclesToDisable.SetActive(false);
        }

        if (_interactionPrompt != null)
        {
            _interactionPrompt.SetActive(false);
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.gray; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_hasBeenPressed)
        {
            _isPlayerNearby = true;
            if (_interactionPrompt != null)
            {
                _interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            if (_interactionPrompt != null)
            {
                _interactionPrompt.SetActive(false);
            }
        }
    }
}