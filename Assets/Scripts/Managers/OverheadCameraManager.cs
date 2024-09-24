using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class OverheadCameraManager : MonoBehaviour {

    [SerializeField] private GameObject overheadCamera;
    [SerializeField] private GameObject playerCamera;

    [SerializeField] private GameState gameState;

    [SerializeField] private BoolVariable canToggleOverheadCameraBool;

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
        canToggleOverheadCameraBool.Value = !gameState.isDialogueRunning && !_ballLaunched;
        if (!canToggleOverheadCameraBool.Value) return;

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