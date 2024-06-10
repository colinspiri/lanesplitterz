using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class BallVFX : MonoBehaviour {
    [SerializeField] private FloatVariable ballFuel;
    
    [SerializeField] private ParticleSystem ballSmokeLeft;
    [SerializeField] private ParticleSystem ballSmokeRight;
    [SerializeField] private ParticleSystem ballSmokeBack;
    [SerializeField] private ParticleSystem ballSmokeFront;

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
            if(!ballSmokeLeft.isPlaying) ballSmokeLeft.Play();
        }
        else ballSmokeLeft.Stop();
        
        if (turnDirection == -1) {
            if(!ballSmokeRight.isPlaying) ballSmokeRight.Play();
        }
        else ballSmokeRight.Stop();
    }

    private void CheckAccelerationDirection() {
        int accelerationDirection = PlayerMovement.Instance.AccelerationDirection;

        if (accelerationDirection == 1) {
            if(!ballSmokeBack.isPlaying) ballSmokeBack.Play();
        }
        else ballSmokeBack.Stop();
        
        if (accelerationDirection == -1) {
            if(!ballSmokeFront.isPlaying) ballSmokeFront.Play();
        }
        else ballSmokeFront.Stop();
    }
}