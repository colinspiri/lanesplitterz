using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTrackTrigger : MonoBehaviour
{
    [SerializeField] private float resetTime;

    private bool _hasPlayerReachedEnd;

    private void OnTriggerEnter(Collider other) {
        if (!_hasPlayerReachedEnd && other.gameObject.CompareTag("Player")) {
            _hasPlayerReachedEnd = true;
            StartCoroutine(ResetCoroutine());
        }
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(resetTime);
        RoundManager.Instance.NotifyBallsAtEndOfTrack();
        _hasPlayerReachedEnd = false;
    }
}
