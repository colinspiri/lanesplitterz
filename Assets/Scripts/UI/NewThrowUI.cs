using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ScriptableObjectArchitecture;

public class NewThrowUI : MonoBehaviour
{
    [SerializeField] private GameObject roundUI;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private float roundUITime;
    [SerializeField] private GameEvent roundUIEndOne;
    [SerializeField] private GameEvent roundUIEndTwo;
    [SerializeField] private GameEvent roundUIEndThree;
    [SerializeField] private GameEvent roundUIEndFour;
    [SerializeField] private GameEvent roundUIEndFive;
    [SerializeField] private GameEvent endWinLoseUI;
    [SerializeField] private GameEvent tutorialReset;
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private IntVariable currentThrow;
    [SerializeField] private IntVariable currentRound;
    [SerializeField] private BoolVariable isClearingPins;
    [SerializeField] private GameObject clearingPinsUI;
    [SerializeField] private GameObject currentScoresUI;
    [SerializeField] private TextMeshProUGUI currentScoresText;
    [SerializeField] private GameObject winLoseUI;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;
    [SerializeField] private GameObject replayTutorialButton;
    [SerializeField] private GameObject playGameButton;
    [SerializeField] private GameObject EndFirstThrowTutorialUI;
    [SerializeField] private GameObject EndSecondThrowTutorialUI;
    [SerializeField] private LaneComponents lane;
    [SerializeField] private Material blueLane;
    [SerializeField] private Material greyLane;

    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    private bool _isFirstThrow;
    private bool _isSecondThrow;

    private void Awake()
    {
        playerInfo.isPracticing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.OnNewRound += () => StartCoroutine(NextRoundUI());
        StartCoroutine(NextRoundUI());
        _isFirstThrow = true;
        _isSecondThrow = false;
    }

    private IEnumerator NextRoundUI()
    {
        if (playerInfo.isPracticing == true) yield break;

        if (currentRound.Value <= 5)
        {
            roundText.text = "Round " + currentRound.Value;
            roundUI.SetActive(true);
            yield return new WaitForSeconds(roundUITime);
            roundUI.SetActive(false);

            if (currentRound.Value == 1) roundUIEndOne.Raise(); // should start dialogue
            if (currentRound.Value == 2) roundUIEndTwo.Raise();
            if (currentRound.Value == 3) roundUIEndThree.Raise();
            if (currentRound.Value == 4) roundUIEndFour.Raise();
            if (currentRound.Value == 5) roundUIEndFive.Raise();

            if (currentRound.Value == 2 || currentRound.Value == 4)
            {
                lane.ground.GetComponent<Renderer>().material = blueLane;
                lane.waitingArea.GetComponent<Renderer>().material = blueLane;
                gameState.isDoublePointsThrow = true;
            }
        }
    }

    private IEnumerator DisplayEndThrowUI()
    {
        if (playerInfo.isPracticing == false) RoundManager.Instance.playerPointsByThrow.Add(playerCurrentPoints.Value);
        if (playerInfo.isPracticing == false) RoundManager.Instance.enemyPointsByThrow.Add(enemyCurrentPoints.Value);
        RoundManager.Instance.CalculateFinalScores();

        if (playerInfo.isPracticing == true)
        {
            StartCoroutine(DisplayClearingPinsUI());
            yield break;
        }

        if (currentThrow.Value % 2 == 0)
        {
            currentScoresText.text = "Your current score:\n" + RoundManager.Instance.playerFinalScore;
            currentScoresUI.SetActive(true);
            yield return new WaitForSeconds(uiConstants.currentScoresTime);
            StartCoroutine(DisplayClearingPinsUI());
        }
        else StartCoroutine(DisplayClearingPinsUI());
    }
    private IEnumerator DisplayClearingPinsUI()
    {

        if (playerInfo.isPracticing == true && _isFirstThrow) EndFirstThrowTutorialUI.SetActive(true);
        if (playerInfo.isPracticing == true && _isSecondThrow) EndSecondThrowTutorialUI.SetActive(true);

        clearingPinsUI.SetActive(true);
        yield return new WaitForSeconds(uiConstants.clearingPinsTime);
        clearingPinsUI.SetActive(false);
        currentScoresUI.SetActive(false);

        if (playerInfo.isPracticing == true && _isSecondThrow) _isSecondThrow = false;

        if (playerInfo.isPracticing == true && _isFirstThrow)
        {
            EndFirstThrowTutorialUI.SetActive(false);
            _isFirstThrow = false;
            _isSecondThrow = true;
        }

        if (playerInfo.isPracticing == true && currentThrow.Value % 2 == 0) DisplayTutorialButtons();
    }

    // call when player wants to play tutorial again
    public void NotifyTutorialReset()
    {
        tutorialReset.Raise();
        clearingPinsUI.SetActive(false);
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;
        isClearingPins.Value = false;
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
        winLoseUI.SetActive(true);

        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore) loseTextObject.SetActive(false);
        else winTextObject.SetActive(false);

        yield return new WaitForSeconds(roundUITime);

        winLoseUI.SetActive(false);
        endWinLoseUI.Raise();
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
        playerInfo.isPracticing = false;
        CallNotifyBallsAtEndOfTrack();
        StopCoroutine(DisplayClearingPinsUI());
        clearingPinsUI.SetActive(false);
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;
        isClearingPins.Value = false;
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
