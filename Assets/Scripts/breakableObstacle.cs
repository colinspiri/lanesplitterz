using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour
{


    void OnTriggerEnter( Collider collider )
    {
        gameObject.SetActive(false);
        //this is where my fuel method would go... if I had one
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = gameObject.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(transform.position, gameObject.transform.localScale);
    }
}
