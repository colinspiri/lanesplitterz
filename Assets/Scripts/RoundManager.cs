using System;
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
        // TODO: scoreboard updates each player's points
        // then a little delay before end of throw (move delay here from end of track trigger)
        
        EndThrow();
    }

    private void EndThrow() {
        if (currentThrow.Value < throwsPerRound && pinsStanding.Count > 0) {
            NextThrow();
        }
        else NextRound();
    }

    private void EndThrowAndRound() {
        NextRound();
    }

    private void NextThrow() {
        currentThrow.Value++;
        
        OnNewThrow?.Invoke();
    }

    private void NextRound() {
        currentRound.Value++;
        currentThrow.Value = 1;
        
        currentPoints.Value = 0;

        OnNewRound?.Invoke();
    }
}