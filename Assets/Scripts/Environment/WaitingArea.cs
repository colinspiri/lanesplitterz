using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class WaitingArea : ActionOnCollide
{
    [Header("Game Events")]
    [SerializeField] private GameEvent startLevelReset;

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
        RoundManager.Instance.CheckElvisAbility();
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

    /// <summary>
    /// Displays the scoreboard and waits for the player to click the button to continue. Once clicked, the game progresses to the next level.
    /// </summary>
    private IEnumerator ResetCoroutine()
    {
        startLevelReset.Raise();
        SetIsClearingPins();

        // when player is still in tutorial, we want reset everything based on the button click, not a wait time
        if (playerInfo.isPracticing == false)
        {
            RoundManager.Instance.UpdateScoreboard();
            yield return new WaitUntil(() => playerInfo.isReady == true);
            playerCurrentPoints.Value = 0;
            enemyCurrentPoints.Value = 0;
            gameState.isClearingPins = false;
            ResetBalls();

            // progresses the game to the next round
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
        }

        // needed for tutorial
        if (playerInfo.isPracticing == true && gameState.currentThrow % 2 == 1)
        {
            yield return new WaitUntil(() => playerInfo.isReady == true);
            RoundManager.Instance.NotifyBallsAtEndOfTrack();
        }
    }

    /// <summary>
    /// Unfreezes the balls and hides them so they can be reset.
    /// </summary>
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

    /// <summary>
    /// Freezes the ball when it collides with the waiting area
    /// </summary>
    /// <param name="ball">Current ball to freeze</param>
    public void Freeze(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (!ballOne) ballOne = ball;
        else if (!ballTwo) ballTwo = ball;
    }

    public void ResetBallCount() => ballCount = 0;
}