using System;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.VFX;

public class BallVFX : MonoBehaviour {
    [SerializeField] private FloatVariable ballFuel;
    
    [SerializeField] private VisualEffect ballSmokeLeft;
    [SerializeField] private VisualEffect ballSmokeRight;
    [SerializeField] private VisualEffect ballSmokeBack;
    [SerializeField] private VisualEffect ballSmokeFront;

    private void Update() {
        if (PlayerMovement.Instance == null || ballFuel.Value <= Mathf.Epsilon) return;
        
        SnapToBall();

        CheckTurnDirection();
        CheckAccelerationDirection();
    }

    private void SnapToBall() {
        transform.position = PlayerMovement.Instance.transform.position;
    }

    private void CheckTurnDirection() {
        int turnDirection = PlayerMovement.Instance.TurnDirection;

        if (turnDirection == 1) {
            ballSmokeLeft.SetUInt("SmokeRate", 10);
        }
        else ballSmokeLeft.SetUInt("SmokeRate", 0);
        
        if (turnDirection == -1) {
            ballSmokeRight.SetUInt("SmokeRate", 10);
        }
        else ballSmokeRight.SetUInt("SmokeRate", 0);
    }

    private void CheckAccelerationDirection() {
        int accelerationDirection = PlayerMovement.Instance.AccelerationDirection;

        if (accelerationDirection == 1) {
            ballSmokeBack.SetUInt("SmokeRate", 10);
        }
        else ballSmokeBack.SetUInt("SmokeRate", 0);
        
        if (accelerationDirection == -1) {
            ballSmokeFront.SetUInt("SmokeRate", 10);
        }
        else ballSmokeFront.SetUInt("SmokeRate", 0);
    }
}