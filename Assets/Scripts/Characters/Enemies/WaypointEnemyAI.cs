using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class WaypointEnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrolling, Searching, Returning }
    public EnemyState _currentState;

    [SerializeField] private Transform[] _patrolWaypoints;
    [SerializeField] private Transform[] _searchWaypoints;
    [SerializeField] private Transform _visionConeTransform;

    [Header("Movimento")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _searchSpeed = 4f;

    private int _currentPatrolIndex = 0;
    private int _currentSearchIndex = 0;
    private Vector2 _originalPosition;
    private EnemyVision _enemyVision;
    private Animator _animator;
    private Vector2 _lastMoveDirection = Vector2.down;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _currentState = EnemyState.Patrolling;

        _enemyVision = GetComponentInChildren<EnemyVision>();

        if (_patrolWaypoints.Length > 0)
        {
            _originalPosition = _patrolWaypoints[0].position;
        }
        else
        {
            _originalPosition = transform.position;
            _currentState = EnemyState.Returning;
            HandleReturning();
        }
    }

    void Update()
    {
        if (_currentState == EnemyState.Patrolling || _currentState == EnemyState.Returning)
        {
            if (_enemyVision != null && _enemyVision.CheckForPlayerLineOfSight())
            {
                PlayerSpotted();
            }
        }

        switch (_currentState)
        {
            case EnemyState.Patrolling: HandlePatrolling(); break;
            case EnemyState.Searching: HandleSearching(); break;
            case EnemyState.Returning: HandleReturning(); break;
        }
    }

    private void HandlePatrolling()
    {
        if (_patrolWaypoints.Length == 0) return;
        Vector2 targetWaypoint = _patrolWaypoints[_currentPatrolIndex].position;
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;
        UpdateAnimatorAndVision(direction, _patrolSpeed);
        MoveTowards(targetWaypoint, _patrolSpeed);
        if (Vector2.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolWaypoints.Length;
        }
    }

    private void HandleSearching()
    {
        if (_searchWaypoints.Length == 0)
        {
            _currentState = EnemyState.Returning;
            EnemyAlertManager.Alert_SearchFinished();
            return;
        }
        Vector2 targetWaypoint = _searchWaypoints[_currentSearchIndex].position;
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;

        UpdateAnimatorAndVision(direction, _searchSpeed);
        MoveTowards(targetWaypoint, _searchSpeed);

        if (Vector2.Distance(transform.position, targetWaypoint) < 0.1f)
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

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            UpdateAnimatorAndVision(Vector2.zero, 0);
            _currentPatrolIndex = 0;
            _currentState = EnemyState.Patrolling;
        }
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    private void UpdateAnimatorAndVision(Vector2 direction, float speed)
    {
        
        if (speed > 0.1f)
        {
            _lastMoveDirection = direction;
        }
        _animator.SetFloat("MoveX", _lastMoveDirection.x);
        _animator.SetFloat("MoveY", _lastMoveDirection.y);

        if (_visionConeTransform != null)
        {
            float angle = Mathf.Atan2(_lastMoveDirection.y, _lastMoveDirection.x) * Mathf.Rad2Deg;
            _visionConeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void PlayerSpotted()
    {
        if (_currentState == EnemyState.Patrolling || _currentState == EnemyState.Returning)
        {
            _currentState = EnemyState.Searching;
            _currentSearchIndex = 0;
            EnemyAlertManager.Alert_PlayerSpotted();
        }
    }
}