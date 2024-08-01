using System;
using System.Collections.Generic;
using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using UnityEngine;
using Yarn.Unity;

public class RoundManager : MonoBehaviour {
    public static RoundManager Instance;

    [Tooltip("Rounds to play before a winner is decided. If -1, game never ends.")]
    [SerializeField] private int totalRounds = -1;
    [SerializeField] private int throwsPerRound = 1;
    
    [Header("Game Events")]
    [SerializeField] private GameEvent ballAtEndOfTrack;
    [SerializeField] private GameEvent endGame;

    [Header("Scriptable Objects")]
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private PinCollection pinsStanding;
    [SerializeField] private LaneComponents lane;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    [Header("Dialogue")] 
    [SerializeField] private string dialogueOnGameStart;
    [SerializeField] private string dialogueOnPlayerWin;
    [SerializeField] private string dialogueOnBossWin;
    [SerializeField] private string dialogueOnRoundTwo;
    [SerializeField] private string dialogueOnRoundThree;
    [SerializeField] private string dialogueOnRoundFour;
    [SerializeField] private string dialogueOnRoundFive;
    private DialogueRunner _dialogueRunner;

    [Space]
    [SerializeField] private AdaptiveMusicContainer gameMusic;
    [SerializeField] private CrowdManager crowdManager;
    [SerializeField] private Material blueLane;
    [SerializeField] private Material greyLane;

    [Space]
    [HideInInspector] public List<int> playerPointsByThrow = new();
    [HideInInspector] public List<int> enemyPointsByThrow = new();
    [HideInInspector] public int playerFinalScore;
    [HideInInspector] public int enemyFinalScore;
    [HideInInspector] public int playerPointsByRound;
    [HideInInspector] public int enemyPointsByRound;

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

        gameState.currentRound = 1;
        gameState.currentThrow = 1;
        gameState.isClearingPins = false;

        isFirstRound = true;
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

    public void PlayCrowdBoo()
    {
        if (playerCurrentPoints == 0)
        {
            crowdManager.CrowdBoo();
        }
    }

    public void NotifyBallsAtEndOfTrack() {
        // then a little delay before end of throw (move delay here from end of track trigger)

        if (gameState.currentThrow == 2) gameState.isScoreboardEnabled = false;
        
        EndThrow();
    }

    public void UpdateScoreboard() {
        if (playerPointsByThrow.Count % 2 == 0)
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
        for (int i = points.Count - 1; i > points.Count - 3; i--) totalPoints += points[i];

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

            NextRound();
        }
        else if (gameState.currentThrow < throwsPerRound) {
            NextThrow();
        }
        else NextRound();
    }

    private void EndThrowAndRound() {
        NextRound();
    }

    private void NextThrow() {
        if ((gameState.currentRound == 2 || gameState.currentRound == 4) && gameState.isDoublePointsThrow == true && gameState.currentThrow == 1)
        {
            lane.ground.GetComponent<Renderer>().material = greyLane;
            lane.waitingArea.GetComponent<Renderer>().material = greyLane;
            gameState.isDoublePointsThrow = false;
        }
        gameState.currentThrow++;
        
        OnNewThrow?.Invoke();
    }

    private void NextRound() {
        if (playerInfo.isPracticing == false && !isFirstRound) gameState.currentRound++;
        if (playerInfo.isPracticing == false) isFirstRound = false;
        gameState.currentThrow = 1;

        if (gameState.currentRound > totalRounds) {
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
        {
            _dialogueRunner.StartDialogue(dialogueOnPlayerWin);
            gameMusic.TransitionSection(0);
        }
            
        else
        {
            _dialogueRunner.StartDialogue(dialogueOnBossWin);
            gameMusic.TransitionSection(1);
        }    
    }
}