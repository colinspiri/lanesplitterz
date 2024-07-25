using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour {
    public static ScoreboardUI Instance;

    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI enemyScoreText;

    [SerializeField] private TextMeshProUGUI playerRoundOneText;
    [SerializeField] private TextMeshProUGUI playerRoundTwoText;
    [SerializeField] private TextMeshProUGUI playerRoundThreeText;
    [SerializeField] private TextMeshProUGUI playerRoundFourText;
    [SerializeField] private TextMeshProUGUI playerRoundFiveText;

    [SerializeField] private TextMeshProUGUI enemyRoundOneText;
    [SerializeField] private TextMeshProUGUI enemyRoundTwoText;
    [SerializeField] private TextMeshProUGUI enemyRoundThreeText;
    [SerializeField] private TextMeshProUGUI enemyRoundFourText;
    [SerializeField] private TextMeshProUGUI enemyRoundFiveText;

    [SerializeField] private IntVariable currentRound;
    [SerializeField] private IntVariable currentThrow;
    [SerializeField] private BoolVariable isPracticing;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerScoreText.text = "";
        enemyScoreText.text = "";
    }

    public void UpdateScoreboardUI() {
/*        playerScoreText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
        enemyScoreText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);*/

        if (isPracticing.Value) return;

        switch(currentRound.Value) {
            case 1:
                if (currentThrow.Value == 1)
                {
                    playerRoundOneText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundOneText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundOneText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundOneText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }
                break;
            case 2:
                if (currentThrow.Value == 1)
                {
                    playerRoundTwoText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundTwoText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundTwoText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundTwoText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }
                break;
            case 3:
                if (currentThrow.Value == 1)
                {
                    playerRoundThreeText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundThreeText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundThreeText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundThreeText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }
                break;
            case 4:
                if (currentThrow.Value == 1)
                {
                    playerRoundFourText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundFourText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundFourText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundFourText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }
                break;
            case 5:
                if (currentThrow.Value == 1)
                {
                    playerRoundFiveText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundFiveText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundFiveText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundFiveText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }
                break;
        }
    }

    public void ShowFinalScores() {
        bool playerWins = RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore;
        playerScoreText.text = (playerWins ? "player won" : "player lost") + " with " +
                               RoundManager.Instance.playerFinalScore + " points";
        enemyScoreText.text = (playerWins ? "enemy lost" : "enemy won") + " with " +
                              RoundManager.Instance.enemyFinalScore + " points";
    }

    private string PointsByThrowToString(IReadOnlyList<int> pointsByThrow) {
        string toString = "";
        for (var i = 0; i < pointsByThrow.Count; i++) {
            var point = pointsByThrow[i];

            //if (i % 2 == 1) toString += "/";
            toString += point;
            //if (i % 2 == 1) toString += "\t";
        }
        return toString;
    }
}