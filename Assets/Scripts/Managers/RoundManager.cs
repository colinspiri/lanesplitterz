using System;
using System.Collections.Generic;
using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using UnityEngine;
using Yarn.Unity;

public class RoundManager : MonoBehaviour {
    public static RoundManager Instance;

    [Tooltip("Rounds to play before a winner is decided. If -1, game never ends.")]
    public int totalRounds = -1;
    public int throwsPerRound = 1;
    
    [Header("Game Events")]
    [SerializeField] private GameEvent ballAtEndOfTrack;
    [SerializeField] private GameEvent endGame;

    [Header("Scriptable Objects")]
    [SerializeField] private IntVariable playerCurrentPoints;
    [SerializeField] private IntVariable enemyCurrentPoints;
    [SerializeField] private PinCollection pinsStanding;
    [SerializeField] private LaneComponents lane;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private ElvisInfo elvisInfo;
    [SerializeField] private GameState gameState;

    [Header("Dialogue")] 
    [SerializeField] private string dialogueOnGameStart;
    [SerializeField] private string dialogueOnPlayerWin;
    [SerializeField] private string dialogueOnBossWin;
    [SerializeField] private string dialogueOnRoundTwo;
    [SerializeField] private string dialogueOnRoundThree;
    [SerializeField] private string dialogueOnRoundFour;
    [SerializeField] private string dialogueOnRoundFive;
    //private DialogueRunner _dialogueRunner;

    [Space]
    [SerializeField] private AdaptiveMusicContainer gameMusic;
    [SerializeField] private CrowdManager crowdManager;
    [SerializeField] private Material blueLane;
    [SerializeField] private Material greyLane;
    [SerializeField] private int maxLevels;

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
        //_dialogueRunner = FindObjectOfType<DialogueRunner>();
    }
    private void Start() {
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;

        gameState.currentRound = 1;
        gameState.currentThrow = 1;
        gameState.isClearingPins = false;

        ChangeLaneMaterial(greyLane);

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

    public void PlayCrowdBoo()
    {
        if (playerCurrentPoints == 0)
        {
            crowdManager.CrowdBoo();
        }
    }

    // This is specifically after the waiting period finishes
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
        if (playerInfo.isPracticing == true) return;
        
        playerPointsByThrow.Add(playerCurrentPoints.Value);
        enemyPointsByThrow.Add(enemyCurrentPoints.Value);

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

    public void CheckElvisAbility()
    {
        if (!gameState.isElvisLevel) return;

        if (elvisInfo.doublePointsThrows.ContainsKey(gameState.currentRound))
        {
            if (elvisInfo.doublePointsThrows[gameState.currentRound] == gameState.currentThrow)
            {
                ChangeLaneMaterial(blueLane);
                gameState.isDoublePointsThrow = true;
                return;
            }
        }

        ChangeLaneMaterial(greyLane);
        gameState.isDoublePointsThrow = false;
    }

    private void ChangeLaneMaterial(Material material)
    {
        lane.ground.GetComponent<Renderer>().material = material;
        lane.waitingArea.GetComponent<Renderer>().material = material;
    }

    private void NextThrow() {
        gameState.currentThrow++;
        CheckElvisAbility();
        OnNewThrow?.Invoke();
    }

    private void NextRound() {
        if (playerInfo.skippedTutorial) gameState.currentRound++;
        else if (playerInfo.isPracticing == false && !isFirstRound) gameState.currentRound++;
        if (playerInfo.isPracticing == false) isFirstRound = false;
        gameState.currentThrow = 1;

        if (gameState.currentRound > totalRounds) {
            ScoreboardUI.Instance.ShowFinalScores();
            gameState.currentRound = 1;
            endGame.Raise();
            return;
        }

        
        OnNewRound?.Invoke();
    }

/*    public void PlayEndDialgue()
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
    }*/

    public void ClearPlayerCurrentPoints()
    {
        playerCurrentPoints.Value = 0;
        enemyCurrentPoints.Value = 0;
    }

    public void ClearFinalScores()
    {
        playerFinalScore = 0;
        enemyFinalScore = 0;
        playerPointsByThrow.Clear();
        enemyPointsByThrow.Clear();
    }
}