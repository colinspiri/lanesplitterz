using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class WaitingArea : ActionOnCollide
{
    //[SerializeField] private float resetTime;
    [SerializeField] private FloatVariable levelResetTime;
    [SerializeField] private IntVariable currentThrow;
    [SerializeField] private GameEvent startLevelReset;
    [SerializeField] private GameEvent endLevelReset;
    [SerializeField] private FloatVariable clearingPinsTime;
    [SerializeField] private FloatVariable currentScoresTime;

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
        if (currentThrow.Value % 2 == 0)
            yield return new WaitForSeconds(clearingPinsTime.Value + currentScoresTime.Value);
        else 
            yield return new WaitForSeconds(clearingPinsTime.Value);
        //yield return new WaitForSeconds(levelResetTime.Value);
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
