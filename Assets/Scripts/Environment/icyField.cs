using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icyField : MonoBehaviour
{
    //sends a log to console if ball is in icy field (might also do the same if any OTHER collider interacts with it, but, ya know)
    private void OnTriggerEnter( Collider collider )
    {      
        //Debug.Log("Icy Field");
    }

    //draws a gizmo over the icy field
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(69, 1, 24));
    }


}
