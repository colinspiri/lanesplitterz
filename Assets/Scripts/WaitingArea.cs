using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class WaitingArea : ActionOnCollide
{
    //[SerializeField] private float resetTime;
    [SerializeField] private FloatVariable levelResetTime;
    [SerializeField] private GameEvent startLevelReset;
    [SerializeField] private GameEvent endLevelReset;

    private int ballCount = 10;
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
            ballCount++;
            StartCoroutine(ResetCoroutine());
        }
    }

    private IEnumerator ResetCoroutine()
    {
        startLevelReset.Raise();
        yield return new WaitForSeconds(levelResetTime.Value);
        ballOne.gameObject.SetActive(false);
        ballTwo.gameObject.SetActive(false);
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

    public void ResetBallCount() => ballCount = 0;
}
