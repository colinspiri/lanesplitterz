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
    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    [Space]
    [SerializeField] private GameObject ground;
    private GameObject ballOne = null;
    private GameObject ballTwo = null;
    private int ballCount = 10;
    private int layer;

    private void Awake()
    {
        RoundManager.Instance.CheckElvisAbility();
        layer = LayerMask.NameToLayer("Balls");
    }

    protected override void DoAction(Collision collision)
    {
        if ((!ballOne || !ballTwo) && collision.gameObject.layer == layer)
        {
            ballCount++;
            if (collision.gameObject.CompareTag("Player")) ballOne = collision.gameObject;
            if (collision.gameObject.CompareTag("Enemy Ball")) ballTwo = collision.gameObject;
            Freeze(collision.gameObject);
        }

        if (ballOne && ballTwo && collision.gameObject.layer == layer)
        {
            ballCount++;
            RoundManager.Instance.PlayCrowdBoo();
            StartCoroutine(ResetCoroutine());
            if (playerInfo.isPracticing == true)
            {
                ballOne = null;
                ballTwo = null;
            }
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
        if (playerInfo.isPracticing == true)
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
        PlayerMovement.Instance.isFrozen = false;
        ballOne?.gameObject?.SetActive(false);
        ballTwo?.gameObject?.SetActive(false);
        ballOne.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ballTwo.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ballOne = null;
        ballTwo = null;
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
        // Rigidbody rb = ball.GetComponent<Rigidbody>();
        // rb.constraints = RigidbodyConstraints.FreezeAll;

        // rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (ball.CompareTag("Player"))
        {
            PlayerMovement.Instance.isFrozen = true;
            PlayerMovement.Instance.Stop();
        }
        else
        {
            ball.GetComponent<EnemyBall>().Stop();
        }
    }

    public void ResetBallCount() => ballCount = 0;
}