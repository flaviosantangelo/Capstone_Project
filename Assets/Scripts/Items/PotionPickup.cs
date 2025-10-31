using UnityEngine;

[RequireComponent(typeof(Collider2D))] 
public class PotionPickup : MonoBehaviour
{
    [Header("Pozione")]
    [SerializeField] private int _healAmount = 25; 
    [SerializeField] private bool _destroyOnPickup = true;
    //[SerializeField] private GameObject _pickupEffect;
    //[SerializeField] private AudioClip _pickupSound;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true; 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerDataManager._instance.Heal(_healAmount);

            if (_destroyOnPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}