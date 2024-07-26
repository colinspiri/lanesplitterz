using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class WaitingArea : ActionOnCollide
{
    //[SerializeField] private float resetTime;
    [SerializeField] private FloatVariable levelResetTime;
    [SerializeField] private IntVariable currentThrow;
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private GameEvent startLevelReset;
    [SerializeField] private GameEvent endLevelReset;
    [SerializeField] private FloatVariable clearingPinsTime;
    [SerializeField] private FloatVariable currentScoresTime;
    [SerializeField] private BoolVariable isPracticing;
    [SerializeField] private BoolVariable isClearingPins;

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
            RoundManager.Instance.PlayCrowdBoo();
            StartCoroutine(ResetCoroutine());
        }
    }

    private IEnumerator ResetCoroutine()
    {
        startLevelReset.Raise();
        SetIsClearingPins();

    // when player is still in tutorial, we want reset everything based on the button click, not a wait time

        if (currentThrow.Value % 2 == 0 && isPracticing.Value == false)
        {
            yield return new WaitForSeconds(clearingPinsTime.Value + currentScoresTime.Value);
            RoundManager.Instance.UpdateScoreboard();
            playerCurrentPoints.Value = 0;
            enemyCurrentPoints.Value = 0;
            isClearingPins.Value = false;
            ResetBalls();
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
        }
        else if (currentThrow.Value % 2 == 1)
        {
            yield return new WaitForSeconds(clearingPinsTime.Value);
            if (isPracticing == false) RoundManager.Instance.UpdateScoreboard();
            ResetBalls();
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
            playerCurrentPoints.Value = 0;
            enemyCurrentPoints.Value = 0;
            isClearingPins.Value = false;
        }

        //yield return new WaitForSeconds(levelResetTime.Value);
        /*        ballOne.gameObject.SetActive(false);
                ballTwo.gameObject.SetActive(false);
                ballOne.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                ballTwo.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;*/

    }

    // for tutorial call when player clicks button to replay tutorial
    public void ResetBalls()
    {
        ballOne.gameObject.SetActive(false);
        ballTwo.gameObject.SetActive(false);
        ballOne.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ballTwo.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public void CallNotifyBallsAtEndOfTrack() => RoundManager.Instance.NotifyBallsAtEndOfTrack();

    public void CallResetPoints()
    {
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;
    }

    public void SetIsClearingPins() => isClearingPins.Value = true;

    public void Freeze(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (!ballOne) ballOne = ball;
        else if (!ballTwo) ballTwo = ball;
    }

    public void ResetBallCount() => ballCount = 0;
}
