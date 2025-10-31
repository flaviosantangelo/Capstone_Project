using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    void Update()
    {

        if (_target != null)
        {
            Vector3 newPosition = _target.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

    }

}
