using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingArea : ActionOnCollide
{
    [SerializeField] private float resetTime;

    private int ballCount = 0;
    private GameObject ballOne = null;
    private GameObject ballTwo = null;
    protected override void DoAction(Collision collision)
    {
        if (ballCount < 2)
        {
            ballCount++;
            Freeze(collision.gameObject);
        }

        if (ballCount == 2)
        {
            StartCoroutine(ResetCoroutine());
            ballCount = 0;
        }
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(resetTime);
        ballOne.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ballTwo.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        RoundManager.Instance.NotifyBallsAtEndOfTrack();
    }

    public void Freeze(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (!ballOne) ballOne = ball;
        else if (!ballTwo) ballTwo = ball;
    }
}
