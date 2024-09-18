using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityEngine.UI;
using Unity.VisualScripting.FullSerializer.Internal;

public class NewThrowUI : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private GameEvent elvisStartDialogueRoundOne;
    [SerializeField] private GameEvent elvisStartDialogueRoundTwo;
    [SerializeField] private GameEvent elvisStartDialogueRoundThree;
    [SerializeField] private GameEvent elvisStartDialogueRoundFour;
    [SerializeField] private GameEvent elvisStartDialogueRoundFive;
    [Space]
    [SerializeField] private GameEvent corpoStartDialogueRoundOne;
    [SerializeField] private GameEvent corpoStartDialogueRoundTwo;
    [SerializeField] private GameEvent corpoStartDialogueRoundThree;
    [SerializeField] private GameEvent corpoStartDialogueRoundFour;
    [SerializeField] private GameEvent corpoStartDialogueRoundFive;
    [Space]
    [SerializeField] private GameEvent caesarStartDialogueRoundOne;
    [SerializeField] private GameEvent caesarStartDialogueRoundTwo;
    [SerializeField] private GameEvent caesarStartDialogueRoundThree;
    [SerializeField] private GameEvent caesarStartDialogueRoundFour;
    [SerializeField] private GameEvent caesarStartDialogueRoundFive;
    [Space]
    [SerializeField] private GameEvent roundUIEnd;
    [SerializeField] private GameEvent ElvisEndWinLoseUI;
    [SerializeField] private GameEvent CorpoEndWinLoseUI;
    [SerializeField] private GameEvent CaesarEndWinLoseUI;
    [SerializeField] private GameEvent tutorialReset;
    [SerializeField] private GameEvent ballsAtEndOfTrack;

    [Header("UI Elements")]
    [SerializeField] private GameObject roundUI;
    [SerializeField] private GameObject clearingPinsUI;
    [SerializeField] private GameObject currentScoresUI;
    [SerializeField] private GameObject winLoseUI;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;
    [SerializeField] private GameObject replayTutorialButton;
    [SerializeField] private GameObject playGameButton;
    [SerializeField] private GameObject EndFirstThrowTutorialUI;
    [SerializeField] private GameObject EndSecondThrowTutorialUI;
    [SerializeField] private Image fadeToBlackUI;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI currentScoresText;

    [Header("Scriptable Objects")]
    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    private bool _isFirstThrow;
    private bool _isSecondThrow;
    private bool _playerReady;

    private void Awake()
    {
        //playerInfo.isPracticing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.OnNewRound += () => StartCoroutine(NextRoundUI());
        StartCoroutine(NextRoundUI());
        _isFirstThrow = true;
        _isSecondThrow = false;
        playerInfo.isReady = false;
        playerInfo.finishedTutorial = false;
    }

    private IEnumerator NextRoundUI()
    {
        yield return new WaitUntil(() => fadeToBlackUI.color.a == 0f);

        //if (gameState.currentLevelIndex >= 15) yield break;
        if (playerInfo.isPracticing == true) yield break;

        if (gameState.currentRound <= RoundManager.Instance.totalRounds)
        {
            gameState.isPauseMenuEnabled = false;
            roundText.text = "Round " + gameState.currentRound;
            roundUI.SetActive(true);
            yield return new WaitForSeconds(uiConstants.roundUITime);
            roundUI.SetActive(false);
            gameState.isPauseMenuEnabled = true;

            // enables cannon input
            roundUIEnd.Raise();

            // TODO: events to start round dialogue for elvis
            if (gameState.currentLevelIndex <= RoundManager.Instance.totalRounds)
            {
                if (gameState.currentRound == 1) elvisStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) elvisStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) elvisStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) elvisStartDialogueRoundFour.Raise();
                if (gameState.currentRound == 5) elvisStartDialogueRoundFive.Raise();
            }
            // TODO: events to start dialogue for corpo
            else if (gameState.currentLevelIndex <= RoundManager.Instance.totalRounds * 2)
            {
                if (gameState.currentRound == 1) corpoStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) corpoStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) corpoStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) corpoStartDialogueRoundFour.Raise();
                if (gameState.currentRound == 5) corpoStartDialogueRoundFive.Raise();
            }
            // TODO: events to start dialogue for ceasar
            else if (gameState.currentLevelIndex <= RoundManager.Instance.totalRounds * 3)
            {
                if (gameState.currentRound == 1) caesarStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) caesarStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) caesarStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) caesarStartDialogueRoundFour.Raise();
                if (gameState.currentRound == 5) caesarStartDialogueRoundFive.Raise();
            }
        }
    }

    private IEnumerator DisplayEndThrowUI()
    {
        ballsAtEndOfTrack.Raise();

        if (playerInfo.isPracticing == true || gameState.currentThrow == 1)
        {
            StartCoroutine(DisplayClearingPinsUI());
            yield break;
        }
        else if (gameState.currentThrow % 2 == 0)
        {
            currentScoresText.text = "Your current score:\n" + RoundManager.Instance.playerFinalScore;
            //currentScoresUI.SetActive(true);
            //yield return new WaitForSeconds(uiConstants.currentScoresTime);
            StartCoroutine(DisplayClearingPinsUI());
            yield break;
        }
    }

    public void RedisplayScoreboard() => StartCoroutine(DisplayClearingPinsUI());
    public void HideScoreboard() => StopAllCoroutines();
    private IEnumerator DisplayClearingPinsUI()
    {
        if (playerInfo.isPracticing == true && _isFirstThrow) EndFirstThrowTutorialUI.SetActive(true);
        if (playerInfo.isPracticing == true && _isSecondThrow) EndSecondThrowTutorialUI.SetActive(true);

        //clearingPinsUI.SetActive(true);

        if (!playerInfo.isPracticing) ScoreboardUI.Instance.DisplayScoreboard();

        if (!playerInfo.isPracticing) yield return new WaitUntil(() => playerInfo.isReady == true);
        else
        {
            yield return new WaitForSeconds(uiConstants.clearingPinsTime);
            SetPlayerReady();
        }

        if (!playerInfo.isPracticing) ScoreboardUI.Instance.HideScoreboard();

        //yield return new WaitForSeconds(uiConstants.clearingPinsTime);
        //clearingPinsUI.SetActive(false);
        //currentScoresUI.SetActive(false);

        if (playerInfo.isPracticing == true && _isSecondThrow) _isSecondThrow = false;

        if (playerInfo.isPracticing == true && _isFirstThrow)
        {
            EndFirstThrowTutorialUI.SetActive(false);
            _isFirstThrow = false;
            _isSecondThrow = true;
        }

        if (playerInfo.isPracticing == true && gameState.currentThrow % 2 == 0) DisplayTutorialButtons();
    }

    public void SetPlayerReady() => playerInfo.isReady = true;

    // call when player wants to play tutorial again
    public void NotifyTutorialReset()
    {
        tutorialReset.Raise();
        clearingPinsUI.SetActive(false);
        gameState.isClearingPins = false;
        RoundManager.OnNewRound.Invoke();
    }

    /*    private IEnumerator DisplayCurrentScores()
        {
            RoundManager.Instance.playerPointsByThrow.Add(playerCurrentPoints.Value);
            RoundManager.Instance.enemyPointsByThrow.Add(enemyCurrentPoints.Value);

            if (currentThrow % 2 == 0)
            {
                RoundManager.Instance.CalculateFinalScores();
                currentScoresText.text = "Player current score: " + RoundManager.Instance.playerFinalScore + "\nEnemy current score: " + RoundManager.Instance.enemyFinalScore;
                currentScoresUI.SetActive(true);
                yield return new WaitForSeconds(currentScoresTime);
                //yield return new WaitForSeconds(levelResetTime.Value);
                //currentScoresUI.SetActive(false);
                currentThrow++;
            }
        }*/

    private IEnumerator DisplayWinLoseUI()
    {
        gameState.isPauseMenuEnabled = false;
        winLoseUI.SetActive(true);

        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore)
        {
            loseTextObject.SetActive(false);
            playerInfo.isWinning = true;
        }
        else
        {
            winTextObject.SetActive(false);
            playerInfo.isWinning = false;
        }

        yield return new WaitForSeconds(uiConstants.roundUITime);

        winLoseUI.SetActive(false);
        loseTextObject.SetActive(true);
        winTextObject.SetActive(true);

        if (gameState.currentLevelIndex == RoundManager.Instance.totalRounds) ElvisEndWinLoseUI.Raise();
        if (gameState.currentLevelIndex == RoundManager.Instance.totalRounds * 2) CorpoEndWinLoseUI.Raise();
        if (gameState.currentLevelIndex == RoundManager.Instance.totalRounds * 3) CaesarEndWinLoseUI.Raise();

        gameState.isPauseMenuEnabled = true;
    }

    public void DisplayTutorialButtons()
    {
        EnableMouse();
        replayTutorialButton.SetActive(true);
        playGameButton.SetActive(true);
    }

    public void HideTutorialButtons()
    {
        DisableMouse();
        replayTutorialButton.SetActive(false);
        playGameButton.SetActive(false);
        EndSecondThrowTutorialUI.SetActive(false);
    }

    public void CallDisplayEndThrowUI() => StartCoroutine(DisplayEndThrowUI());

    public void CallDisplayWinLoseUI() => StartCoroutine(DisplayWinLoseUI());

    public void EndTutorial()
    {
        playerInfo.finishedTutorial = true;
        playerInfo.isPracticing = false;
        CallNotifyBallsAtEndOfTrack();
        StopCoroutine(DisplayClearingPinsUI());
        clearingPinsUI.SetActive(false);
        RoundManager.Instance.ClearPlayerCurrentPoints();
        gameState.isClearingPins = false;
    }

    public void CallNotifyBallsAtEndOfTrack() => RoundManager.Instance.NotifyBallsAtEndOfTrack();

    public void EnableMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
