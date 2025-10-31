using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [Header("Impostazioni Salute")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }
    public void TakeDamage(int damageAmount)
    {
        if (_currentHealth <= 0) return; 

        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
}