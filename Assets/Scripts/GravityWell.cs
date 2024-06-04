using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMove;
    [SerializeField] private float liftVelocity = 20f;

    void OnTriggerEnter( Collider col )
    {
        _playerMove.GetComponent<Rigidbody>().AddForce(transform.up * liftVelocity);
    }

}
