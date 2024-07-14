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
            ballSmokeLeft.Play();
            ballSmokeLeft.SetUInt("SmokeRate", 100);
        }
        else {
            ballSmokeLeft.Stop();
        } 
        
        if (turnDirection == -1) {
            ballSmokeRight.Play();
            ballSmokeRight.SetUInt("SmokeRate", 100);
        }
        else {
            ballSmokeRight.Stop();
        }
    }

    private void CheckAccelerationDirection() {
        int accelerationDirection = PlayerMovement.Instance.AccelerationDirection;

        if (accelerationDirection == 1) {
            ballSmokeBack.Play();
            ballSmokeBack.SetUInt("SmokeRate", 100);
        }
        else {
            ballSmokeBack.Stop();
        }

        if (accelerationDirection == -1) {
            ballSmokeFront.Play();
            ballSmokeFront.SetUInt("SmokeRate", 100);
        }
        else {
            ballSmokeFront.Stop();
        }
    }
}