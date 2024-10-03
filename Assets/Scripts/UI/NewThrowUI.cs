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
    [SerializeField] private GameEvent elvisStartDialogueIntermission;
    [Space]
    [SerializeField] private GameEvent corpoStartDialogueRoundOne;
    [SerializeField] private GameEvent corpoStartDialogueRoundTwo;
    [SerializeField] private GameEvent corpoStartDialogueRoundThree;
    [SerializeField] private GameEvent corpoStartDialogueRoundFour;
    [SerializeField] private GameEvent corpoStartDialogueRoundFive;
    [SerializeField] private GameEvent corpoStartDialogueIntermission;
    [Space]
    [SerializeField] private GameEvent caesarStartDialogueRoundOne;
    [SerializeField] private GameEvent caesarStartDialogueRoundTwo;
    [SerializeField] private GameEvent caesarStartDialogueRoundThree;
    [SerializeField] private GameEvent caesarStartDialogueRoundFour;
    [SerializeField] private GameEvent caesarStartDialogueRoundFive;
    [SerializeField] private GameEvent caesarStartDialogueIntermission;
    [Space]
    [SerializeField] private GameEvent startTutorialDialogue;
    [SerializeField] private GameEvent startRoundTutorialDialogue;
    [SerializeField] private GameEvent startEndTutorialDialogue;
    [Space]
    [SerializeField] private GameEvent roundUIEnd;
    [SerializeField] private GameEvent ElvisEndWinLoseUI;
    [SerializeField] private GameEvent CorpoEndWinLoseUI;
    [SerializeField] private GameEvent CaesarEndWinLoseUI;
    [SerializeField] private GameEvent tutorialReset;
    [SerializeField] private GameEvent ballsAtEndOfTrack;

    [Header("UI Elements")]
    [SerializeField] private GameObject roundUI;
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

    [Header("Scriptable Objects")]
    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    private bool _isFirstThrow;
    private bool _isSecondThrow;

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.OnNewRound += () => StartCoroutine(NextRoundUI());
        StartCoroutine(NextRoundUI());
        _isFirstThrow = true;
        gameState.isTutorialFirstThrow = true;
        gameState.isTutorialSecondThrow = false;
        _isSecondThrow = false;
        playerInfo.isReady = false;
        playerInfo.finishedTutorial = false;
    }

    /// <summary>
    /// Displays round number UI and raises events to start dialogue
    /// </summary>
    private IEnumerator NextRoundUI()
    {
        yield return new WaitUntil(() => fadeToBlackUI.color.a == 0f);

        if (playerInfo.isPracticing == true)
        {
            if (_isFirstThrow) startTutorialDialogue.Raise();
            yield break;
        }

        if (gameState.currentRound <= RoundManager.Instance.totalRounds)
        {
            gameState.isPauseMenuEnabled = false;
            if (gameState.currentRound == 4)
            {
                roundText.text = "Halftime";
            }
            else
            {
                roundText.text = "Round " + gameState.currentRound;
            }
            roundUI.SetActive(true);
            yield return new WaitForSeconds(uiConstants.roundUITime);
            roundUI.SetActive(false);
            gameState.isPauseMenuEnabled = true;

            // enables cannon input
            roundUIEnd.Raise();

            if (gameState.isElvisLevel)
            {
                if (gameState.currentRound == 1) elvisStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) elvisStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) elvisStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) elvisStartDialogueIntermission.Raise();
                if (gameState.currentRound == 5) elvisStartDialogueRoundFive.Raise();
            }
            else if (gameState.isCorpoLevel)
            {
                if (gameState.currentRound == 1) corpoStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) corpoStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) corpoStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) corpoStartDialogueIntermission.Raise();
                if (gameState.currentRound == 5) corpoStartDialogueRoundFive.Raise();
            }
            else if (gameState.isCaesarLevel)
            {
                if (gameState.currentRound == 1) caesarStartDialogueRoundOne.Raise();
                if (gameState.currentRound == 2) caesarStartDialogueRoundTwo.Raise();
                if (gameState.currentRound == 3) caesarStartDialogueRoundThree.Raise();
                if (gameState.currentRound == 4) caesarStartDialogueIntermission.Raise();
                if (gameState.currentRound == 5) caesarStartDialogueRoundFive.Raise();
            }
        }
    }

    public void CallStartRoundFour() => StartCoroutine(StartRoundFour());

    /// <summary>
    /// Starts round four after the end intermission event is raised
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartRoundFour()
    {
        gameState.isPauseMenuEnabled = false;
        roundText.text = "Round " + gameState.currentRound;
        roundUI.SetActive(true);
        yield return new WaitForSeconds(uiConstants.roundUITime);
        roundUI.SetActive(false);
        gameState.isPauseMenuEnabled = true;

        // enables cannon input
        roundUIEnd.Raise();

        if (gameState.isElvisLevel)
        {
            elvisStartDialogueRoundFour.Raise();
        }
        else if (gameState.isCorpoLevel)
        {
            corpoStartDialogueRoundFour.Raise();
        }
        else if (gameState.isCaesarLevel)
        {
            caesarStartDialogueRoundFour.Raise();
        }
    }

    private void DisplayEndThrowUI()
    {
        ballsAtEndOfTrack.Raise();
        StartCoroutine(DisplayClearingPinsUI());
    }

    public void RedisplayScoreboard() => StartCoroutine(DisplayClearingPinsUI());
    public void HideScoreboard() => StopAllCoroutines();

    /// <summary>
    /// Displays the scoreboard until the player is ready to move on, and displays tutorial UI during the tutorial
    /// </summary>
    private IEnumerator DisplayClearingPinsUI()
    {
        if (playerInfo.isPracticing == true && _isFirstThrow) startRoundTutorialDialogue.Raise();
        if (playerInfo.isPracticing == true && _isSecondThrow) startEndTutorialDialogue.Raise();

        if (!playerInfo.isPracticing) ScoreboardUI.Instance.DisplayScoreboard();

        if (!playerInfo.isPracticing) yield return new WaitUntil(() => playerInfo.isReady == true);
        else if (!_isFirstThrow && !_isSecondThrow)
        {
            if (gameState.currentThrow == 2) DisplayTutorialButtons();
            else
            {
                gameState.isPauseMenuEnabled = false;
                yield return new WaitForSeconds(2f);
                SetPlayerReady();
                gameState.isPauseMenuEnabled = true;
            }
        }

        if (!playerInfo.isPracticing) ScoreboardUI.Instance.HideScoreboard();

        if (playerInfo.isPracticing == true && _isSecondThrow)
        {
            _isSecondThrow = false;
            gameState.isTutorialSecondThrow = false;
        }

        if (playerInfo.isPracticing == true && _isFirstThrow)
        {
            EndFirstThrowTutorialUI.SetActive(false);
            _isFirstThrow = false;
            gameState.isTutorialFirstThrow = false;
            _isSecondThrow = true;
            gameState.isTutorialSecondThrow = true;
        }
    }

    public void SetPlayerReady() => playerInfo.isReady = true;

    // call when player wants to play tutorial again
    public void NotifyTutorialReset()
    {
        tutorialReset.Raise();
        gameState.isClearingPins = false;
    }

    /// <summary>
    /// Displays win/lose UI and raises events to play win/lose dialogue
    /// </summary>
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

    public void CallDisplayEndThrowUI() => DisplayEndThrowUI();

    public void CallDisplayWinLoseUI() => StartCoroutine(DisplayWinLoseUI());

    public void EndTutorial()
    {
        playerInfo.finishedTutorial = true;
        playerInfo.isPracticing = false;
        StopCoroutine(DisplayClearingPinsUI());
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