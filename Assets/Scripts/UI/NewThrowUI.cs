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
    [SerializeField] private GameEvent roundUIEnd;
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private FloatVariable levelResetTime; // use this in Waiting Area script
    [SerializeField] private GameObject clearingPinsUI;
    [SerializeField] private GameObject currentScoresUI;
    [SerializeField] private TextMeshProUGUI currentScoresText;

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

            if (currentRound == 1) roundUIEnd.Raise(); // should start dialogue
        }
    }

    private IEnumerator DisplayClearingPinsUI()
    {
        if (currentThrow % 2 == 1)
        {
            clearingPinsUI.SetActive(true);
            yield return new WaitForSeconds(levelResetTime.Value);
            clearingPinsUI.SetActive(false);
            currentThrow++;
        }
    }

    private IEnumerator DisplayCurrentScores()
    {
        RoundManager.Instance.playerPointsByThrow.Add(playerCurrentPoints.Value);
        RoundManager.Instance.enemyPointsByThrow.Add(enemyCurrentPoints.Value);

        if (currentThrow % 2 == 0)
        {
            RoundManager.Instance.CalculateFinalScores();
            currentScoresText.text = "Player current score: " + RoundManager.Instance.playerFinalScore + "\nEnemy current score: " + RoundManager.Instance.enemyFinalScore;
            currentScoresUI.SetActive(true);
            yield return new WaitForSeconds(levelResetTime.Value);
            currentScoresUI.SetActive(false);
            currentThrow++;
        }
    }

    public void CallDisplayClearingPins() => StartCoroutine(DisplayClearingPinsUI());

    public void CallDisplayCurrentScores() => StartCoroutine(DisplayCurrentScores());
}
