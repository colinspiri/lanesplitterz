using System;
using UnityEngine;

public class BallVFX : MonoBehaviour {
    [SerializeField] private ParticleSystem turningSmokeLeft;
    [SerializeField] private ParticleSystem turningSmokeRight;

    private void Update() {
        if (PlayerMovement.Instance == null) return;
        
        SnapToBall();

        UpdateTurningSmoke();
    }

    private void SnapToBall() {
        transform.position = PlayerMovement.Instance.transform.position;
    }

    private void UpdateTurningSmoke() {
        int turnDirection = PlayerMovement.Instance.TurnDirection;

        if (turnDirection == 1) {
            if(!turningSmokeLeft.isPlaying) turningSmokeLeft.Play();
        }
        else turningSmokeLeft.Stop();
        
        if (turnDirection == -1) {
            if(!turningSmokeRight.isPlaying) turningSmokeRight.Play();
        }
        else turningSmokeRight.Stop();
    }
}