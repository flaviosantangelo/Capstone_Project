using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    public float _moveSpd = 5f;
    private Rigidbody2D _rb;
    private Vector2 _rb_moveDirection; 

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("Mover non ha un Rigidbody", this);
        }
        else
        {
            _rb.freezeRotation = true; 

            if (_rb.bodyType == RigidbodyType2D.Dynamic)
            {
                _rb.gravityScale = 0;
            }
        }
    }
    public void SetMoveDirection(Vector2 direction) 
    {
        _rb_moveDirection = direction.normalized;
    }

    void FixedUpdate()
    {
        if (_rb != null)
        {
            Vector2 targetVelocity = _rb_moveDirection * _moveSpd;
            _rb.velocity = targetVelocity;
        }
    }
   
    public void Stop()
    {
        SetMoveDirection(Vector2.zero); 
    }
    
    public bool IsMoving()
    {
        return _rb_moveDirection.sqrMagnitude > 0;
    }
    public void SetSpeed(float newSpeed)
    {
        _moveSpd = newSpeed;
    }
    
    public void Warp(Vector2 position) 
    {
        if (_rb != null)
        {
            _rb.position = position;
        }
    }
}