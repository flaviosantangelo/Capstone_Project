using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(UniqueID))] 
public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    private UniqueID _uniqueID; 

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _uniqueID = GetComponent<UniqueID>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDataManager._instance.AddCoins(_coinValue);
            PlayerDataManager._instance.ReportItemCollected(_uniqueID.GetID());
            Destroy(gameObject);
        }
    }
}