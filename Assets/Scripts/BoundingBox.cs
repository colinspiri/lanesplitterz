using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    [SerializeField] private float resetTime;

    private bool _isOutOfBounds;

    private void OnTriggerExit(Collider other)
    {
        if (!_isOutOfBounds && other.gameObject.CompareTag("Player"))
        {
            _isOutOfBounds = true;
            StartCoroutine(ResetCoroutine());
        }
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(resetTime);
        RoundManager.Instance.NotifyBallsAtEndOfTrack();
        _isOutOfBounds = false;
    }
}
