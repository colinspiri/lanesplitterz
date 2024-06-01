using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTrackTrigger : MonoBehaviour
{
    [SerializeField] private float resetTime;
    private int _ballCount = 0;
    private int _ballLayer;

    private void Start()
    {
        _ballLayer = LayerMask.NameToLayer("Balls");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _ballLayer)
        {
            _ballCount++;
            // if (ballCount == 2) // this means both balls have reached the end of the track
            if (_ballCount > 0) // temp until we have an enemy ball
            {
                _ballCount = 0;
                StartCoroutine(ResetCoroutine());
            }
        }
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(3);
        RoundManager.Instance.NotifyBallsAtEndOfTrack();
    }
}
