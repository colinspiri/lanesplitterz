using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMove;
    [SerializeField] private float SecondsStunned = 0.5f;

    //When Ball hits billboard, activates Stun.
    IEnumerator OnTriggerEnter( Collider Hit )
    {
        StartCoroutine( EnableStun() );
        yield return null;
    }

    //Stun mechanic (disable movement -> wait for SecondsStunned -> re-enable movement)
    IEnumerator EnableStun ()
    {
        playerMove.GetComponent<PlayerMovement>().acceptingInputs = false;
        Debug.Log("Seconds Stunned: " + SecondsStunned);
        yield return new WaitForSeconds( SecondsStunned );
        playerMove.GetComponent<PlayerMovement>().acceptingInputs = true;
        Debug.Log("Unstunned!"); 
        yield return null;
    }

}
