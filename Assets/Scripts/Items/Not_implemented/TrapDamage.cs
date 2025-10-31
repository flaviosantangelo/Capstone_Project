using UnityEngine;

[RequireComponent(typeof(Collider2D))] 
public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float _dmgAmount = 10f;
    [SerializeField] private bool _oneTimeUse = false;
    private bool _hasBeenTriggered = false;
    [SerializeField] private GameObject _trapVisuals;
    [SerializeField] private bool _startHidden = true;
    private bool _isRevealed = false;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true; 
        if (_startHidden && _trapVisuals != null)
        {
            _trapVisuals.SetActive(false);
            _isRevealed = false;
        }
        else
        {
            _isRevealed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (_oneTimeUse && _hasBeenTriggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            int damageAsInt = (int)_dmgAmount;
            PlayerDataManager._instance.TakeDamage(damageAsInt);
            _hasBeenTriggered = true;

            if (_oneTimeUse)
            {
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !_oneTimeUse)
        {
            _hasBeenTriggered = false; 
        }
    }

    public void RevealTrap()
    {
        if (_trapVisuals != null && !_isRevealed)
        {
            _trapVisuals.SetActive(true);
            _isRevealed = true;
        }
    }
}