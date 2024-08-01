using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class WaitingArea : ActionOnCollide
{
    [Header("Game Events")]
    [SerializeField] private GameEvent startLevelReset;
    [SerializeField] private GameEvent endLevelReset;

    [Header("Scriptable Objects")]
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private LaneComponents lane;
    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    [Space]
    [SerializeField] private GameObject ground;
    private GameObject ballOne = null;
    private GameObject ballTwo = null;
    private int ballCount = 10;

    private void Awake()
    {
        lane.ground = ground;
        lane.waitingArea = gameObject;
    }

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

        if (gameState.currentThrow % 2 == 0 && playerInfo.isPracticing == false)
        {
            yield return new WaitForSeconds(uiConstants.clearingPinsTime + uiConstants.currentScoresTime);
            RoundManager.Instance.UpdateScoreboard();
            playerCurrentPoints.Value = 0;
            enemyCurrentPoints.Value = 0;
            gameState.isClearingPins = false;
            ResetBalls();
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
        }
        else if (gameState.currentThrow % 2 == 1)
        {
            yield return new WaitForSeconds(uiConstants.clearingPinsTime);
            if (playerInfo.isPracticing == false) RoundManager.Instance.UpdateScoreboard();
            ResetBalls();
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
            playerCurrentPoints.Value = 0;
            enemyCurrentPoints.Value = 0;
            gameState.isClearingPins = false;
        }
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

    public void SetIsClearingPins() => gameState.isClearingPins = true;

    public void Freeze(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (!ballOne) ballOne = ball;
        else if (!ballTwo) ballTwo = ball;
    }

    public void ResetBallCount() => ballCount = 0;
}
