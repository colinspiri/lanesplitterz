using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour
{
    private PlayerMovement _playerMove;
    [Range(0.01f, 1f)]
    [SerializeField] private float fuelSub = 0.1f;
    
    void OnTriggerEnter( Collider collider )
    {
        gameObject.SetActive(false);
        _playerMove.ReduceFuel(fuelSub);

        //this is where my fuel method would go... if I had one
    }

    private void OnDrawGizmos()
    {
        //Gizmos.matrix = gameObject.transform.localToWorldMatrix;;
        //Gizmos.DrawWireCube(transform.position, gameObject.transform.localScale);
    }
}
