using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icyField : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMove;

   private void Update()
    {
        if ( playerMove.isIcy() )
        {
            playerMove.Turn(0f);
        }
    }



    private void OnTriggerEnter( Collider collider )
    {      
        //playerMove.Turn (0f, true);
        Debug.Log("Turning Off");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(70, 1, 50));
    }


}
