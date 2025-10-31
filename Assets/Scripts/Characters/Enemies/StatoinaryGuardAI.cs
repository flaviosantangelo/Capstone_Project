using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class StationaryGuardAI : MonoBehaviour
{
    public enum EnemyState { Guarding, Searching, Returning }
    public EnemyState _currentState;

    [Header("Riferimenti")]
    [SerializeField] private Transform[] _searchWaypoints;
    [SerializeField] private Transform _visionConeTransform;

    [Header("Guardia")]
    [SerializeField] private float _minGuardTime = 1.0f;
    [SerializeField] private float _maxGuardTime = 5.0f;

    [Header("Movimento")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _searchSpeed = 4f;

    private int _currentSearchIndex = 0;
    private Vector2 _originalPosition;
    private Coroutine _guardCoroutine;
    private EnemyVision _enemyVision;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemyVision = GetComponentInChildren<EnemyVision>();
        _originalPosition = transform.position;
        StartGuarding();
    }

    void Update()
    {
        if (_currentState == EnemyState.Guarding)
        {
            if (_enemyVision != null && _enemyVision.CheckForPlayerLineOfSight())
            {
                PlayerSpotted();
            }
        }
        switch (_currentState)
        {
            case EnemyState.Guarding: break;
            case EnemyState.Searching: HandleSearching(); break;
            case EnemyState.Returning: HandleReturning(); break;
        }
    }

    private void StartGuarding()
    {
        _currentState = EnemyState.Guarding;
        if (_guardCoroutine != null) StopCoroutine(_guardCoroutine);
        _guardCoroutine = StartCoroutine(GuardFacingCoroutine());
    }

    private void HandleSearching()
    {
        if (_searchWaypoints.Length == 0)
        {
            _currentState = EnemyState.Returning;
            EnemyAlertManager.Alert_SearchFinished();
            return;
        }
        Vector2 target = _searchWaypoints[_currentSearchIndex].position;
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        UpdateAnimatorAndVision(direction, _searchSpeed);
        MoveTowards(target, _searchSpeed);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            _currentSearchIndex++;
            if (_currentSearchIndex >= _searchWaypoints.Length) _currentState = EnemyState.Returning;
            EnemyAlertManager.Alert_SearchFinished();
        }
    }

    private void HandleReturning()
    {
        Vector2 target = _originalPosition;
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        UpdateAnimatorAndVision(direction, _patrolSpeed); 
        MoveTowards(target, _patrolSpeed);

        if (Vector2.Distance(transform.position, target) < 0.1f) StartGuarding();
    }
    
    private IEnumerator GuardFacingCoroutine()
    {
        
        SetIdleDirection(Vector2.down);
        while (_currentState == EnemyState.Guarding)
        {
            float waitTime = Random.Range(_minGuardTime, _maxGuardTime);
            yield return new WaitForSeconds(waitTime);

            Vector2 idleDirection = Vector2.zero;

            switch (Random.Range(0, 4))
            {
                case 0: idleDirection = Vector2.down; break;
                case 1: idleDirection = Vector2.up; break;
                case 2: idleDirection = Vector2.left; break;
                case 3: idleDirection = Vector2.right; break;
            }

            SetIdleDirection(idleDirection);
        }
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
   
    private void SetIdleDirection(Vector2 direction)
    {
        _animator.SetFloat("Speed", 0); 
        _animator.SetFloat("MoveX", direction.x); 
        _animator.SetFloat("MoveY", direction.y); 

  
        if (_visionConeTransform != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _visionConeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
   
    private void UpdateAnimatorAndVision(Vector2 direction, float speed)
    {
        _animator.SetFloat("Speed", speed); 
        _animator.SetFloat("MoveX", direction.x);
        _animator.SetFloat("MoveY", direction.y);

        if (_visionConeTransform != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _visionConeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public void PlayerSpotted()
    {
        if (_currentState == EnemyState.Guarding)
        {
            if (_guardCoroutine != null) StopCoroutine(_guardCoroutine);
            _currentState = EnemyState.Searching;
            _currentSearchIndex = 0;
            EnemyAlertManager.Alert_PlayerSpotted();
        }
    }
}