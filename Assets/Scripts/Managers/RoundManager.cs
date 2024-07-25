using System;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using Yarn.Unity;

public class RoundManager : MonoBehaviour {
    public static RoundManager Instance;

    [Tooltip("Rounds to play before a winner is decided. If -1, game never ends.")]
    [SerializeField] private int totalRounds = -1;
    [SerializeField] private int throwsPerRound = 1;
    [SerializeField] private IntVariable currentRound;
    [SerializeField] private IntVariable currentThrow;
    
    [Space]
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private BoolVariable isPracticing;
    [SerializeField] private PinCollection pinsStanding;
    [SerializeField] private GameEvent ballAtEndOfTrack;
    [SerializeField] private GameEvent endGame;

    [Space] 
    [SerializeField] private string dialogueOnGameStart;
    [SerializeField] private string dialogueOnPlayerWin;
    [SerializeField] private string dialogueOnBossWin;
    [SerializeField] private string dialogueOnRoundTwo;
    [SerializeField] private string dialogueOnRoundThree;
    [SerializeField] private string dialogueOnRoundFour;
    [SerializeField] private string dialogueOnRoundFive;
    private DialogueRunner _dialogueRunner;

    [Space] 
    public List<int> playerPointsByThrow = new();
    public List<int> enemyPointsByThrow = new();
    public int playerFinalScore;
    public int enemyFinalScore;

    [HideInInspector]
    public int playerPointsByRound;
    [HideInInspector]
    public int enemyPointsByRound;

    public static Action OnNewThrow;
    public static Action OnNewRound;

    private bool isFirstRound;


    private void Awake() {
        Instance = this;
        _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }
    private void Start() {
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;

        currentRound.Value = 1;
        currentThrow.Value = 1;

        isFirstRound = true;

/*        if (_dialogueRunner && dialogueOnGameStart != "") {
            _dialogueRunner.StartDialogue(dialogueOnGameStart);
        }*/
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            EndThrow();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            EndThrowAndRound();
        }
    }

    public void PlayStartDialogue()
        {
        if (_dialogueRunner && dialogueOnGameStart != "") {
            _dialogueRunner.StartDialogue(dialogueOnGameStart);
        }
    }

    public void PlayPlayerWinDialogue()
            {
        if (_dialogueRunner && dialogueOnPlayerWin != "") {
            _dialogueRunner.StartDialogue(dialogueOnPlayerWin);
        }
    }

    public void PlayBossWinDialogue()
            {
        if (_dialogueRunner && dialogueOnBossWin != "") {
            _dialogueRunner.StartDialogue(dialogueOnBossWin);
        }
    }

    public void PlayRoundTwoDialogue()
            {
        if (_dialogueRunner && dialogueOnRoundTwo != "") {
            _dialogueRunner.StartDialogue(dialogueOnRoundTwo);
        }
    }

    public void PlayRoundThreeDialogue()
            {
        if (_dialogueRunner && dialogueOnRoundThree != "") {
            _dialogueRunner.StartDialogue(dialogueOnRoundThree);
        }
    }

    public void PlayRoundFourDialogue()
            {
        if (_dialogueRunner && dialogueOnRoundFour != "") {
            _dialogueRunner.StartDialogue(dialogueOnRoundFour);
        }
    }

    public void PlayRoundFiveDialogue()
            {
        if (_dialogueRunner && dialogueOnRoundFive != "") {
            _dialogueRunner.StartDialogue(dialogueOnRoundFive);
        }
    }

    public void NotifyBallsAtEndOfTrack() {
        //UpdateScoreboard();
        
        // then a little delay before end of throw (move delay here from end of track trigger)
        
        EndThrow();
        //ballAtEndOfTrack.Raise();
    }

    public void UpdateScoreboard() {
        playerPointsByThrow.Add(playerCurrentPoints.Value);
        enemyPointsByThrow.Add(enemyCurrentPoints.Value);

        if (playerPointsByThrow.Count == 2)
        {
            playerPointsByRound = CalculatePointsByRound(playerPointsByThrow);
            enemyPointsByRound = CalculatePointsByRound(enemyPointsByThrow);
        }

        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;

        if(ScoreboardUI.Instance) ScoreboardUI.Instance.UpdateScoreboardUI();
    }

    public int CalculatePointsByRound(List<int> points)
    {
        int totalPoints = 0;
        foreach (var point in points) totalPoints += point;

        return totalPoints;
    }

    public void CalculateFinalScores() {
        playerFinalScore = 0;
        foreach (var playerPoint in playerPointsByThrow) {
            playerFinalScore += playerPoint;
        }

        enemyFinalScore = 0;
        foreach (var enemyPoint in enemyPointsByThrow) {
            enemyFinalScore += enemyPoint;
        }
    }

    private void EndThrow() {
        // if no more pins, count points as 0 and skip to next round
        if (pinsStanding.Count == 0) {
            playerPointsByThrow.Add(0);
            enemyPointsByThrow.Add(0);
            if(ScoreboardUI.Instance) ScoreboardUI.Instance.UpdateScoreboardUI();

            NextRound();
        }
        else if (currentThrow.Value < throwsPerRound) {
            NextThrow();
        }
        else NextRound();
    }

    private void EndThrowAndRound() {
        //UpdateScoreboard();
        NextRound();
    }

    private void NextThrow() {
        currentThrow.Value++;
        
        OnNewThrow?.Invoke();
    }

    private void NextRound() {
        if (isPracticing.Value == false && !isFirstRound) currentRound.Value++;
        if (isPracticing.Value == false) isFirstRound = false;
        currentThrow.Value = 1;

        if (currentRound.Value > totalRounds) {
            CalculateFinalScores();
            ScoreboardUI.Instance.ShowFinalScores();
            endGame.Raise();
            return;
        }
        
        OnNewRound?.Invoke();
    }

    public void PlayEndDialgue()
    {
       if (playerFinalScore > enemyFinalScore)
            _dialogueRunner.StartDialogue(dialogueOnPlayerWin);
        else
            _dialogueRunner.StartDialogue(dialogueOnBossWin);
    }
}