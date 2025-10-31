using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private Mover _mover;
    [SerializeField] private float _walkSpd = 5f;
    [SerializeField] private float _runSpd = 8f;
    private Camera _mainCamera;

    [Header("Attacco Normale")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.25f;

    [Header("Attacco Potente")]
    [SerializeField] private GameObject _powerfulBulletPrefab;
    [SerializeField] private float _chargeShotTime = 3.0f;
    [SerializeField] private int powerfulShotDmg = 40;

    private float _chargeShotTimer = 0f;
    private bool _isCharging = false;
    private bool _isCharged = false;

    [Header("Scene escluse")]
    [SerializeField] private List<string> _nonGameplayScenes = new List<string> { "MainMenu", "ShopScene" };
    private bool _canShoot = true;

    private Animator _animator;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection; 
    private float _fireTimer = 0f;

    void Start()
    {
        _mover = GetComponent<Mover>();
        _animator = GetComponent<Animator>();
        _lastMoveDirection = Vector2.down; 
        _mainCamera = Camera.main;
        string currentScene = SceneManager.GetActiveScene().name;
        if (_nonGameplayScenes.Contains(currentScene))
        {
            _canShoot = false;
            _mover.SetSpeed(_walkSpd);
        }
        else
        {
            _canShoot = true;
            _mover.SetSpeed(_walkSpd);
            if (PoolingManager._instance != null)
            {
                if (_bulletPrefab != null) PoolingManager._instance.CreatePool(_bulletPrefab, 20);
                if (_powerfulBulletPrefab != null) PoolingManager._instance.CreatePool(_powerfulBulletPrefab, 5);
            }
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector2(moveX, moveY);

        if (moveX != 0)
        {
            _moveInput.y = 0; 
        }
        _moveInput.Normalize(); 

        if (_moveInput.sqrMagnitude > 0.1f)
        {
            _lastMoveDirection = _moveInput;
        }
        
        if (!_canShoot)
        {
            HandleSprinting();
            _animator.SetFloat("speed", _moveInput.sqrMagnitude * _mover._moveSpd);
            _animator.SetFloat("moveX", _lastMoveDirection.x);
            _animator.SetFloat("moveY", _lastMoveDirection.y);
            return;
        }
        HandleSprinting();
        HandleShooting();
        HandleChargedShot();

        _animator.SetFloat("speed", _moveInput.sqrMagnitude * _mover._moveSpd);
        _animator.SetFloat("moveX", _lastMoveDirection.x);
        _animator.SetFloat("moveY", _lastMoveDirection.y);

        if (Input.GetKeyDown(KeyCode.H)) PlayerDataManager._instance.UsePotion();
    }

    private void HandleSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _moveInput.magnitude > 0.1f)
            _mover.SetSpeed(_runSpd);
        else
            _mover.SetSpeed(_walkSpd);
        _mover.SetMoveDirection(_moveInput);
    }

    private void HandleShooting()
    {
        if (_fireTimer < _fireRate) _fireTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && !_isCharging)
        {
            if (_fireTimer >= _fireRate)
            {
                _fireTimer = 0f;
                FireBullet(false);
            }
        }
    }
    private void HandleChargedShot()
    {
        if (Input.GetMouseButtonDown(1)) { _isCharging = true; _isCharged = false; _chargeShotTimer = 0f; }
        if (Input.GetMouseButton(1) && _isCharging)
        {
            _chargeShotTimer += Time.deltaTime;
            if (_chargeShotTimer >= _chargeShotTime && !_isCharged) _isCharged = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (_isCharged) FireBullet(true);
            _isCharging = false; _isCharged = false; _chargeShotTimer = 0f;
        }
    }

    private void FireBullet(bool isPowerful)
    {
        Vector2 fireDirection = _lastMoveDirection;
        GameObject prefabToSpawn = isPowerful ? _powerfulBulletPrefab : _bulletPrefab;

        if (prefabToSpawn == null || _firePoint == null)
        {
            return;
        }

        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        _firePoint.rotation = Quaternion.Euler(0, 0, angle);
        GameObject bulletGO = PoolingManager._instance.Spawn(prefabToSpawn, _firePoint.position, _firePoint.rotation);
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        if (bulletScript != null && isPowerful)
        {
            bulletScript.SetDamage(powerfulShotDmg);
        }
    }
}