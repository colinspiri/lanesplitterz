using System;
using UnityEngine;

public class OverheadCameraManager : MonoBehaviour {

    [SerializeField] private GameObject overheadCamera;
    [SerializeField] private GameObject playerCamera;

    [SerializeField] private GameState gameState;

    private bool _ballLaunched;
    private bool _overheadViewEnabled;

    private void Start() {
        overheadCamera.SetActive(false);
        
        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
    }

    private void Initialize() {
        _ballLaunched = false;
        _overheadViewEnabled = false;
        SetOverheadCamera(_overheadViewEnabled);
    }

    private void Update() {
        if (gameState.isDialogueRunning || _ballLaunched) return;

        if (Input.GetKeyDown(KeyCode.Tab)) {
            SetOverheadCamera(!_overheadViewEnabled);
        }
    }

    private void SetOverheadCamera(bool value) {
        _overheadViewEnabled = value;

        overheadCamera.SetActive(_overheadViewEnabled);
        playerCamera.SetActive(!_overheadViewEnabled);
    }

    // called from LaunchedBall GameEvent SO
    public void NotifyBallLaunched() {
        _ballLaunched = true;
        SetOverheadCamera(false);
    }
}