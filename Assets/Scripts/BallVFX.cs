using System;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.VFX;

public class BallVFX : MonoBehaviour {
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameObject ballSpin;
    [SerializeField] private VisualEffect ballSmokeLeft;
    [SerializeField] private VisualEffect ballSmokeRight;
    [SerializeField] private VisualEffect ballSmokeBack;
    [SerializeField] private VisualEffect ballSmokeFront;

    private void Start() {
        ballSpin.SetActive(false);
    }

    private void Update() {
        if (PlayerMovement.Instance == null || playerInfo.currentFuel <= Mathf.Epsilon) return;
        
        SnapToBall();
        ballSpin.transform.position = PlayerMovement.Instance.transform.position;

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
            ballSpin.SetActive(true);
        }
        else {
            ballSmokeLeft.Stop();
            ballSpin.SetActive(false);
        } 
        
        if (turnDirection == -1) {
            ballSmokeRight.Play();
            ballSpin.SetActive(true);
        }
        else {
            ballSmokeRight.Stop();
            ballSpin.SetActive(false);
        }
    }

    private void CheckAccelerationDirection() {
        int accelerationDirection = PlayerMovement.Instance.AccelerationDirection;

        if (accelerationDirection == 1) {
            ballSmokeBack.Play();
        }
        else {
            ballSmokeBack.Stop();
        }

        if (accelerationDirection == -1) {
            ballSmokeFront.Play();
        }
        else {
            ballSmokeFront.Stop();
        }
    }
}