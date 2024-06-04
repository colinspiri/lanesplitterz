using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMove;
    [SerializeField] public GameObject billboard;
    [SerializeField] public GameObject movementPoint1;
    [SerializeField] public GameObject movementPoint2;
    [SerializeField] private AudioSource destroySound;
    [SerializeField] public float moveDuration = 5;
    [SerializeField] private float slowDown = 0.01f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            yield return StartCoroutine( MoveBillboardForward ( movementPoint2.transform.position ) );
            yield return StartCoroutine( MoveBillboardBack ( movementPoint1.transform.position ) );
        }
    }

    //Moves billboard from one position to another (via lerping)
    IEnumerator MoveBillboardForward ( Vector3 targetPosition )
    {
        Vector3 startPosition = movementPoint1.transform.position;
        float timeElapsed = 0;
        
        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed/moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    IEnumerator MoveBillboardBack ( Vector3 targetPosition )
    {
        Vector3 startPosition = movementPoint2.transform.position;
        float timeElapsed = 0;
        
        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed/moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    //draws movement points for easy understanding
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(movementPoint1.transform.position, new Vector3(12, 5, 1));
        Gizmos.DrawWireCube(movementPoint2.transform.position, new Vector3(12, 5, 1));
    }

    //on trigger hit, slows down Ball by a float and deactivates the object
    void OnTriggerEnter( Collider billboardHit )
    {
        billboard.gameObject.SetActive(false);
        playerMove.Accelerate( slowDown );
        destroySound.Play();
    }

}
