using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour {
    public static ScoreboardUI Instance;

    [Header("Score Text")]
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI enemyScoreText;

    [Header("Total Scores")]
    [SerializeField] private TextMeshProUGUI playerTotalScoreText;
    [SerializeField] private TextMeshProUGUI enemyTotalScoreText;

    [Header("Player Scoreboard Score Text")]
    [SerializeField] private TextMeshProUGUI playerRoundOneText;
    [SerializeField] private TextMeshProUGUI playerRoundTwoText;
    [SerializeField] private TextMeshProUGUI playerRoundThreeText;
    [SerializeField] private TextMeshProUGUI playerRoundFourText;
    [SerializeField] private TextMeshProUGUI playerRoundFiveText;

    [Header("Enemy Scoreboard Score Text")]
    [SerializeField] private TextMeshProUGUI enemyRoundOneText;
    [SerializeField] private TextMeshProUGUI enemyRoundTwoText;
    [SerializeField] private TextMeshProUGUI enemyRoundThreeText;
    [SerializeField] private TextMeshProUGUI enemyRoundFourText;
    [SerializeField] private TextMeshProUGUI enemyRoundFiveText;

    [Header("Player Scoreboard White Pin Text")]
    [SerializeField] private TextMeshProUGUI playerRoundOneWhiteText;
    [SerializeField] private TextMeshProUGUI playerRoundTwoWhiteText;
    [SerializeField] private TextMeshProUGUI playerRoundThreeWhiteText;
    [SerializeField] private TextMeshProUGUI playerRoundFourWhiteText;
    [SerializeField] private TextMeshProUGUI playerRoundFiveWhiteText;

    [Header("Player Scoreboard Gold Pin Text")]
    [SerializeField] private TextMeshProUGUI playerRoundOneGoldText;
    [SerializeField] private TextMeshProUGUI playerRoundTwoGoldText;
    [SerializeField] private TextMeshProUGUI playerRoundThreeGoldText;
    [SerializeField] private TextMeshProUGUI playerRoundFourGoldText;
    [SerializeField] private TextMeshProUGUI playerRoundFiveGoldText;

    [Header("Enemy Scoreboard White Pin Text")]
    [SerializeField] private TextMeshProUGUI enemyRoundOneWhiteText;
    [SerializeField] private TextMeshProUGUI enemyRoundTwoWhiteText;
    [SerializeField] private TextMeshProUGUI enemyRoundThreeWhiteText;
    [SerializeField] private TextMeshProUGUI enemyRoundFourWhiteText;
    [SerializeField] private TextMeshProUGUI enemyRoundFiveWhiteText;

    [Header("Enemy Scoreboard Gold Pin Text")]
    [SerializeField] private TextMeshProUGUI enemyRoundOneGoldText;
    [SerializeField] private TextMeshProUGUI enemyRoundTwoGoldText;
    [SerializeField] private TextMeshProUGUI enemyRoundThreeGoldText;
    [SerializeField] private TextMeshProUGUI enemyRoundFourGoldText;
    [SerializeField] private TextMeshProUGUI enemyRoundFiveGoldText;

    [Space]
    [SerializeField] private TextMeshProUGUI enemyTitleText;

    [Space]
    [SerializeField] private GameObject menuManager;
    [SerializeField] private GameObject scoreboardUI;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    private List<TextMeshProUGUI> whiteAndGoldTexts = new();
    private List<TextMeshProUGUI> scoreTexts = new();

    private int playerWhitePinCountFirstThrow;
    private int playerGoldPinCountFirstThrow;
    private int enemyWhitePinCountFirstThrow;
    private int enemyGoldPinCountFirstThrow;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerScoreText.text = "";
        enemyScoreText.text = "";

        whiteAndGoldTexts.Add(playerRoundOneWhiteText);
        whiteAndGoldTexts.Add(playerRoundTwoWhiteText);
        whiteAndGoldTexts.Add(playerRoundThreeWhiteText);
        whiteAndGoldTexts.Add(playerRoundFourWhiteText);
        whiteAndGoldTexts.Add(playerRoundFiveWhiteText);
        whiteAndGoldTexts.Add(playerRoundOneGoldText);
        whiteAndGoldTexts.Add(playerRoundTwoGoldText);
        whiteAndGoldTexts.Add(playerRoundThreeGoldText);
        whiteAndGoldTexts.Add(playerRoundFourGoldText);
        whiteAndGoldTexts.Add(playerRoundFiveGoldText);
        whiteAndGoldTexts.Add(enemyRoundOneWhiteText);
        whiteAndGoldTexts.Add(enemyRoundTwoWhiteText);
        whiteAndGoldTexts.Add(enemyRoundThreeWhiteText);
        whiteAndGoldTexts.Add(enemyRoundFourWhiteText);
        whiteAndGoldTexts.Add(enemyRoundFiveWhiteText);
        whiteAndGoldTexts.Add(enemyRoundOneGoldText);
        whiteAndGoldTexts.Add(enemyRoundTwoGoldText);
        whiteAndGoldTexts.Add(enemyRoundThreeGoldText);
        whiteAndGoldTexts.Add(enemyRoundFourGoldText);
        whiteAndGoldTexts.Add(enemyRoundFiveGoldText);

        scoreTexts.Add(playerRoundOneText);
        scoreTexts.Add(playerRoundTwoText);
        scoreTexts.Add(playerRoundThreeText);
        scoreTexts.Add(playerRoundFourText);
        scoreTexts.Add(playerRoundFiveText);
        scoreTexts.Add(enemyRoundOneText);
        scoreTexts.Add(enemyRoundTwoText);
        scoreTexts.Add(enemyRoundThreeText);
        scoreTexts.Add(enemyRoundFourText);
        scoreTexts.Add(enemyRoundFiveText);

        gameState.isScoreboardEnabled = false;
    }

    private void Update()
    {
        //if (playerInfo.isPracticing == false && Input.GetKeyDown(KeyCode.Tab) && gameState.isScoreboardEnabled == true) scoreboardUI.SetActive(true);
        //if (playerInfo.isPracticing == false && Input.GetKeyUp(KeyCode.Tab)) scoreboardUI.SetActive(false);
    }

    public void DisplayScoreboard()
    {
        menuManager.SetActive(true);
        scoreboardUI.SetActive(true);
        continueButton.SetActive(true);
        gameState.isScoreboardEnabled = true;
    }
    public void HideScoreboard()
    {
        menuManager.SetActive(false);
        scoreboardUI.SetActive(false);
        continueButton.SetActive(false);
        gameState.isScoreboardEnabled = false;
    }

    private string ChangeScoreboardRoundText(List<int> pointsByThrow)
    {
        return pointsByThrow[^1].ToString();
    }

    public void UpdateScoreboardUI() {
/*        playerScoreText.text = PointsByThrowToString(RoundManager.Instance.playerPointsByThrow);
        enemyScoreText.text = PointsByThrowToString(RoundManager.Instance.enemyPointsByThrow);*/

        if (playerInfo.isPracticing) return;

        switch(gameState.currentRound) {
            case 1:
                if (gameState.currentThrow == 1)
                {
                    playerRoundOneText.text = ChangeScoreboardRoundText(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundOneText.text = ChangeScoreboardRoundText(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundOneText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundOneText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }

                ChangePinText(playerRoundOneWhiteText, PinManager.Instance.playerWhitePinCount);
                ChangePinText(playerRoundOneGoldText, PinManager.Instance.playerGoldPinCount);
                ChangePinText(enemyRoundOneWhiteText, PinManager.Instance.enemyWhitePinCount);
                ChangePinText(enemyRoundOneGoldText, PinManager.Instance.enemyGoldPinCount);

                break;
            case 2:
                if (gameState.currentThrow == 1)
                {
                    playerRoundTwoText.text = ChangeScoreboardRoundText(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundTwoText.text = ChangeScoreboardRoundText(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundTwoText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundTwoText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }

                ChangePinText(playerRoundTwoWhiteText, PinManager.Instance.playerWhitePinCount);
                ChangePinText(playerRoundTwoGoldText, PinManager.Instance.playerGoldPinCount);
                ChangePinText(enemyRoundTwoWhiteText, PinManager.Instance.enemyWhitePinCount);
                ChangePinText(enemyRoundTwoGoldText, PinManager.Instance.enemyGoldPinCount);

                break;
            case 3:
                if (gameState.currentThrow == 1)
                {
                    playerRoundThreeText.text = ChangeScoreboardRoundText(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundThreeText.text = ChangeScoreboardRoundText(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundThreeText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundThreeText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }

                ChangePinText(playerRoundThreeWhiteText, PinManager.Instance.playerWhitePinCount);
                ChangePinText(playerRoundThreeGoldText, PinManager.Instance.playerGoldPinCount);
                ChangePinText(enemyRoundThreeWhiteText, PinManager.Instance.enemyWhitePinCount);
                ChangePinText(enemyRoundThreeGoldText, PinManager.Instance.enemyGoldPinCount);

                break;
            case 4:
                if (gameState.currentThrow == 1)
                {
                    playerRoundFourText.text = ChangeScoreboardRoundText(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundFourText.text = ChangeScoreboardRoundText(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundFourText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundFourText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }

                ChangePinText(playerRoundFourWhiteText, PinManager.Instance.playerWhitePinCount);
                ChangePinText(playerRoundFourGoldText, PinManager.Instance.playerGoldPinCount);
                ChangePinText(enemyRoundFourWhiteText, PinManager.Instance.enemyWhitePinCount);
                ChangePinText(enemyRoundFourGoldText, PinManager.Instance.enemyGoldPinCount);

                break;
            case 5:
                if (gameState.currentThrow == 1)
                {
                    playerRoundFiveText.text = ChangeScoreboardRoundText(RoundManager.Instance.playerPointsByThrow);
                    enemyRoundFiveText.text = ChangeScoreboardRoundText(RoundManager.Instance.enemyPointsByThrow);
                }
                else
                {
                    playerRoundFiveText.text = RoundManager.Instance.playerPointsByRound.ToString();
                    enemyRoundFiveText.text = RoundManager.Instance.enemyPointsByRound.ToString();
                }

                ChangePinText(playerRoundFiveWhiteText, PinManager.Instance.playerWhitePinCount);
                ChangePinText(playerRoundFiveGoldText, PinManager.Instance.playerGoldPinCount);
                ChangePinText(enemyRoundFiveWhiteText, PinManager.Instance.enemyWhitePinCount);
                ChangePinText(enemyRoundFiveGoldText, PinManager.Instance.enemyGoldPinCount);

                break;
        }

        RoundManager.Instance.playerPointsByRound = 0;
        RoundManager.Instance.enemyPointsByRound = 0;
    }

    public void UpdateTotalScores()
    {
        playerTotalScoreText.text = RoundManager.Instance.playerFinalScore.ToString();
        enemyTotalScoreText.text = RoundManager.Instance.enemyFinalScore.ToString();
    }

    public void ClearScoreboard()
    {
        foreach (TextMeshProUGUI text in scoreTexts) text.text = "-";
        foreach (TextMeshProUGUI text in whiteAndGoldTexts) text.text = "-";
        playerTotalScoreText.text = "-";
        enemyTotalScoreText.text = "-";
    }

    public void UpdateEnemyTitleText()
    {
        if (gameState.currentLevelIndex == 11) enemyTitleText.text = "Caesar";
        else if (gameState.currentLevelIndex == 6) enemyTitleText.text = "Kaiba";
    }

    public void ShowFinalScores() {
        bool playerWins = RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore;
        playerScoreText.text = (playerWins ? "player won" : "player lost") + " with " +
                               RoundManager.Instance.playerFinalScore + " points";
        enemyScoreText.text = (playerWins ? "enemy lost" : "enemy won") + " with " +
                              RoundManager.Instance.enemyFinalScore + " points";
    }

    public void HideFinalScores()
    {
        playerScoreText.text = "";
        enemyScoreText.text = "";
    }

    private void ChangePinText(TextMeshProUGUI text, int pinCount) => text.text = pinCount.ToString();

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

public enum PinColorType
{
    PlayerWhite,
    PlayerGold,
    EnemyWhite,
    EnemyGold
}