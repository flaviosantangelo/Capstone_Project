using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Impostazioni Proiettile")]
    [SerializeField] private float _speed = 15f;
    [SerializeField] private int _dmg = 20; 
    [SerializeField] private float _lifeTime = 3.0f;

    private Rigidbody2D _rb;
    private int _currentDamage; 

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnEnable()
    {
        _rb.velocity = transform.right * _speed;
        _currentDamage = _dmg;
        Invoke("ReturnToPool", _lifeTime);
    }

    private void ReturnToPool()
    {
        PoolingManager._instance.ReturnToPool(gameObject);
    }
    public void SetDamage(int newDamage)
    {
        _currentDamage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(_currentDamage);
        }

        if (!other.CompareTag("Player") && !other.CompareTag("Bullet") && !other.isTrigger)
        {
            CancelInvoke("ReturnToPool");
            ReturnToPool();
        }
    }
}