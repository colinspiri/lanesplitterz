using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardMovement : MonoBehaviour
{
    [SerializeField] public GameObject movementPoint1;
    [SerializeField] public GameObject movementPoint2;
    [SerializeField] public float moveDuration = 5;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            yield return StartCoroutine( MoveBillboardForward ( movementPoint2.transform.position ) );
            yield return StartCoroutine( MoveBillboardBack ( movementPoint1.transform.position ) );
        }
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(movementPoint1.transform.position, new Vector3(12, 5, 1));
        Gizmos.DrawWireCube(movementPoint2.transform.position, new Vector3(12, 5, 1));
    }
}
