using UnityEngine;


public class TurretEnemyAI : MonoBehaviour
{
    [Header("Riferimenti")]
    protected Transform _playerTransform;
    public GameObject _bulletPrefab; 
    public Transform _firePoint;

    [Header("Visione")]
    public float _visionRange = 20f;
    [Range(0f, 360f)]
    public float _visionAngle = 90f;
    public LayerMask _visionObstacleMask; 

    [Header("Attacco")]
    public float _fireRate = 1.0f; 
    private float _fireTimer = 0f;
    [SerializeField] private Vector2 _forwardAxis = Vector2.right; 

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (_playerTransform == null) return;
        _fireTimer += Time.deltaTime;

        if (CanSeePlayer())
        {
            Vector2 lookDir = (Vector2)_playerTransform.position - (Vector2)transform.position; 
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            if (_forwardAxis == Vector2.up)
            {
                angle -= 90f; 
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            if (_fireTimer >= 1f / _fireRate)
            {
                Shoot();
                _fireTimer = 0f;
            }
        }
    }

    void Shoot()
    {
        if (_bulletPrefab == null || _firePoint == null) return;

        Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
    }

    protected bool CanSeePlayer()
    {
        if (_playerTransform == null) return false;
        Vector2 directionToPlayer = (Vector2)_playerTransform.position - (Vector2)transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer > _visionRange)
        {
            return false;
        }
        Vector2 turretForward = transform.TransformDirection(_forwardAxis); 
        float angle = Vector2.Angle(turretForward, directionToPlayer.normalized);
        if (angle > _visionAngle / 2f)
        {
            return false;
        }
        Vector2 origin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, directionToPlayer.normalized, distanceToPlayer, _visionObstacleMask);

        if (hit.collider != null)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return false; 
            }
        }
        return true; 
    }
}