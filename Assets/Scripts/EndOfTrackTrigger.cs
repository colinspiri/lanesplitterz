using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTrackTrigger : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private float resetTime;
    private int ballCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballCount++;
            // if (ballCount == 2) // this means both balls have reached the end of the track
            if (ballCount > 0) // temp until we have an enemy ball
            {
                ballCount = 0;
                StartCoroutine(resetCoroutine());
            }
        }
    }

    private IEnumerator resetCoroutine()
    {
        yield return new WaitForSeconds(3);
        sceneLoader.Restart();
    }
}
