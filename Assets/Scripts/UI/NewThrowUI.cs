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
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private FloatVariable clearingPinsTime; // use this in Waiting Area script
    [SerializeField] private FloatVariable currentScoresTime;
    [SerializeField] private GameObject clearingPinsUI;
    [SerializeField] private GameObject currentScoresUI;
    [SerializeField] private TextMeshProUGUI currentScoresText;
    [SerializeField] private GameObject winLoseUI;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;

    private int currentRound = 0;
    private int currentThrow = 1;

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.OnNewRound += () => StartCoroutine(NextRoundUI());
        StartCoroutine(NextRoundUI());
    }

    private IEnumerator NextRoundUI()
    {
        currentRound++;

        if (currentRound <= 5)
        {
            roundText.text = "Round " + currentRound;
            roundUI.SetActive(true);
            yield return new WaitForSeconds(roundUITime);
            roundUI.SetActive(false);

            if (currentRound == 1) roundUIEndOne.Raise(); // should start dialogue
            if (currentRound == 2) roundUIEndTwo.Raise();
            if (currentRound == 3) roundUIEndThree.Raise();
            if (currentRound == 4) roundUIEndFour.Raise();
            if (currentRound == 5) roundUIEndFive.Raise();
        }
    }

    private IEnumerator DisplayEndThrowUI()
    {
        RoundManager.Instance.playerPointsByThrow.Add(playerCurrentPoints.Value);
        RoundManager.Instance.enemyPointsByThrow.Add(enemyCurrentPoints.Value);

        if (currentThrow % 2 == 0)
        {
            RoundManager.Instance.CalculateFinalScores();
            currentScoresText.text = "Your current score:\n" + RoundManager.Instance.playerFinalScore;
            currentScoresUI.SetActive(true);
            yield return new WaitForSeconds(currentScoresTime.Value);
            StartCoroutine(DisplayClearingPinsUI());
        }
        else StartCoroutine(DisplayClearingPinsUI());

        //currentThrow++;
    }
    private IEnumerator DisplayClearingPinsUI()
    {
/*        if (currentThrow % 2 == 1)
        {*/
            clearingPinsUI.SetActive(true);
            yield return new WaitForSeconds(clearingPinsTime.Value);
            clearingPinsUI.SetActive(false);
            currentScoresUI.SetActive(false);
            currentThrow++;
        //}
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

    //public void CallDisplayClearingPins() => StartCoroutine(DisplayClearingPinsUI());

    //public void CallDisplayCurrentScores() => StartCoroutine(DisplayCurrentScores());
    public void CallDisplayEndThrowUI() => StartCoroutine(DisplayEndThrowUI());

    public void CallDisplayWinLoseUI() => StartCoroutine(DisplayWinLoseUI());
}
