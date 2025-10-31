using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int _attackDmg = 10;
    [SerializeField] private float _attackRate = 1f;
    private float nextAttackTime = 0f;
    private WaypointEnemyAI _waypointAI;
    private StationaryGuardAI _stationaryAI;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _waypointAI = GetComponentInParent<WaypointEnemyAI>();
        _stationaryAI = GetComponentInParent<StationaryGuardAI>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextAttackTime) return;
        
        bool isSearching = false;
        
        if (_waypointAI != null)
        {
            isSearching = (_waypointAI._currentState == WaypointEnemyAI.EnemyState.Searching);
        }
        else if (_stationaryAI != null)
        {
            isSearching = (_stationaryAI._currentState == StationaryGuardAI.EnemyState.Searching);
        }

        if (isSearching)
        {
            PlayerDataManager._instance.TakeDamage(_attackDmg);
            nextAttackTime = Time.time + 1f / _attackRate;
        }
    }
}