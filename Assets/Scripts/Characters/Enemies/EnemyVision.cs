using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Impostazioni Vista")]
    [SerializeField] private float _visionRange = 10f;
    [SerializeField][Range(0, 360)] private float _visionAngle = 90f;
    [SerializeField][Range(3, 21)] private int _numberOfRays = 7;

    [Header("Rilevamento")]
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _playerLayer;
    private LayerMask _finalRaycastMask; 
    private int _enemyLayerMask;

    void Start()
    {
        _finalRaycastMask = _obstacleLayer | _playerLayer;
        _enemyLayerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
    }

    public bool CheckForPlayerLineOfSight()
    {
        float startAngle = -_visionAngle / 2;
        float angleStep = _visionAngle / (_numberOfRays - 1);

        Vector2 origin = transform.parent.position;
        Vector2 coneForward = transform.right; 

        for (int i = 0; i < _numberOfRays; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * coneForward;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, _visionRange, _finalRaycastMask & _enemyLayerMask);

            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & _playerLayer) != 0)
                {
                    Debug.DrawRay(origin, direction * hit.distance, Color.green);
                    return true; 
                }

                if (((1 << hit.collider.gameObject.layer) & _obstacleLayer) != 0)
                {
                    Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * _visionRange, Color.red);
            }
        }
        return false;
    }
}