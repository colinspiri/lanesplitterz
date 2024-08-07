using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBall : MonoBehaviour
{
    private PlayerMovement playerMove;
    [SerializeField] private float SecondsStunned = 0.5f;

    void Start()
    {
        playerMove = PlayerMovement.Instance;

        if (!playerMove) Debug.LogError("SpeedPlane Error: No PlayerMovement found");
    }

    //When Ball hits billboard, activates Stun.
    IEnumerator OnTriggerEnter( Collider Hit )
    {
        if (Hit.gameObject.layer == LayerMask.NameToLayer("Balls"))
        {
            StartCoroutine( EnableStun() );
            yield return null;
        }
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
