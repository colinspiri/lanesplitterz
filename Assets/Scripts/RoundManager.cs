using System;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class RoundManager : MonoBehaviour {
    public static RoundManager Instance;
    
    [SerializeField] private int throwsPerRound = 1;
    [SerializeField] private IntVariable currentRound;
    [SerializeField] private IntVariable currentThrow;
    
    [Space]
    [SerializeField] private IntVariable currentPoints;
    [SerializeField] private PinCollection pinsStanding;

    [Space] 
    public List<int> playerPointsByThrow = new();
    public List<int> enemyPointsByThrow = new();

    public static Action OnNewThrow;
    public static Action OnNewRound;

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        currentPoints.Value = 0;

        currentRound.Value = 1;
        currentThrow.Value = 1;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            EndThrow();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            EndThrowAndRound();
        }
    }

    public void NotifyBallsAtEndOfTrack() {
        UpdateScoreboard();
        
        // then a little delay before end of throw (move delay here from end of track trigger)
        
        EndThrow();
    }

    private void UpdateScoreboard() {
        playerPointsByThrow.Add(currentPoints.Value);
        enemyPointsByThrow.Add(0);
        
        currentPoints.Value = 0;

        if(ScoreboardUI.Instance) ScoreboardUI.Instance.UpdateScoreboardUI();
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
        
        OnNewRound?.Invoke();
    }
}