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
    [SerializeField] private PinCollection pinsStanding;
    [SerializeField] private GameEvent ballAtEndOfTrack;

    [Space] 
    [SerializeField] private string dialogueOnGameStart;
    [SerializeField] private string dialogueOnGameEnd;
    private DialogueRunner _dialogueRunner;

    [Space] 
    public List<int> playerPointsByThrow = new();
    public List<int> enemyPointsByThrow = new();
    public int playerFinalScore;
    public int enemyFinalScore;

    public static Action OnNewThrow;
    public static Action OnNewRound;


    private void Awake() {
        Instance = this;
        _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }
    private void Start() {
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;

        currentRound.Value = 1;
        currentThrow.Value = 1;

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

    public void PlayEndDialogue()
            {
        if (_dialogueRunner && dialogueOnGameEnd != "") {
            _dialogueRunner.StartDialogue(dialogueOnGameEnd);
        }
    }

    public void NotifyBallsAtEndOfTrack() {
        UpdateScoreboard();
        
        // then a little delay before end of throw (move delay here from end of track trigger)
        
        EndThrow();
        //ballAtEndOfTrack.Raise();
    }

    private void UpdateScoreboard() {
        playerPointsByThrow.Add(playerCurrentPoints.Value);
        enemyPointsByThrow.Add(enemyCurrentPoints.Value);
        
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;

        if(ScoreboardUI.Instance) ScoreboardUI.Instance.UpdateScoreboardUI();
    }

    private void CalculateFinalScores() {
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
        UpdateScoreboard();
        NextRound();
    }

    private void NextThrow() {
        currentThrow.Value++;
        
        OnNewThrow?.Invoke();
    }

    private void NextRound() {
        currentRound.Value++;
        currentThrow.Value = 1;

        if (currentRound.Value > totalRounds) {
            CalculateFinalScores();
            ScoreboardUI.Instance.ShowFinalScores();
            _dialogueRunner.StartDialogue(dialogueOnGameEnd);
        }
        
        OnNewRound?.Invoke();
    }
}