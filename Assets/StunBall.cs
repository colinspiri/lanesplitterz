using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMove;
    [SerializeField] private float SecondsStunned = 0.5f;

    IEnumerator OnTriggerEnter( Collider billboardHit )
    {
        StartCoroutine( EnableStun() );
        yield return null;
    }

    IEnumerator EnableStun ()
    {
        playerMove.GetComponent<PlayerMovement>().acceptingInputs = false;
        Debug.Log("Seconds Stunned: " + SecondsStunned);
        yield return new WaitForSeconds(1);
        playerMove.GetComponent<PlayerMovement>().acceptingInputs = true;
        Debug.Log("Unstunned!"); 
        yield return null;
    }

}
