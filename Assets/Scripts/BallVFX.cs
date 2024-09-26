using System;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.VFX;

public class BallVFX : MonoBehaviour {
    [SerializeField] private PlayerInfo playerInfo;
    // [SerializeField] private GameObject ballSpin;
    [SerializeField] private VisualEffect ballSmokeLeft;
    [SerializeField] private VisualEffect ballSmokeRight;
    [SerializeField] private VisualEffect ballSmokeBack;
    [SerializeField] private VisualEffect ballSmokeFront;

    [SerializeField] private GameObject oilSlick;

    private ParticleSystem oilVFX;

    ParticleSystemRenderer ballSpinRenderer;
    private void Start() {
        // ballSpin.GetComponent<ParticleSystem>().Stop();
        // ballSpin.GetComponent<ParticleSystem>().Play();
        // // disable renderer of ballspin
        // ballSpinRenderer = ballSpin.GetComponent<ParticleSystemRenderer>();
        // ballSpinRenderer.enabled = true;
        oilVFX = oilSlick.GetComponent<ParticleSystem>();
        oilVFX.Play();
    }

    private void Update() {
        if (PlayerMovement.Instance == null || playerInfo.currentFuel <= Mathf.Epsilon) return;
        SnapToBall();
        // CheckSpin();
        CheckTurnDirection();
        CheckAccelerationDirection();
        if (PlayerMovement.Instance.IsIcy() && oilVFX.isPlaying == false) {
            oilVFX.Play();
        }
        else if (!PlayerMovement.Instance.IsIcy() && oilVFX.isPlaying == true) {
            oilVFX.Stop();
        }
    }

    private void SnapToBall() {
        transform.position = PlayerMovement.Instance.transform.position;
        // ballSpin.transform.position = PlayerMovement.Instance.transform.position - new Vector3(0, 0, 2.0f);
    }

    // private void CheckSpin()
    // {
        // if (PlayerMovement.Instance.TurnDirection != 0)
        // {
        //     ballSpinRenderer.enabled = true;
        // }
        // else
        // {
        //     ballSpinRenderer.enabled = false;
        // }
    // }

    private void CheckTurnDirection() {
        int turnDirection = PlayerMovement.Instance.TurnDirection;

        if (turnDirection == 1) {
            ballSmokeLeft.Play();
        }
        else {
            ballSmokeLeft.Stop();
        } 
        
        if (turnDirection == -1) {
            ballSmokeRight.Play();
        }
        else {
            ballSmokeRight.Stop();
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

    // void OnTriggerEnter(Collider other) {
    //     if (other.gameObject.CompareTag("Icy") && oilVFX.isPlaying == false) {
    //         Debug.Log("Icy");
    //         oilVFX.Play();
    //     }
    // }

    // void OnTriggerExit(Collider other) {
    //     if (other.gameObject.CompareTag("Icy") && oilVFX.isPlaying == true) {
    //         Debug.Log("Not Icy");
    //         oilVFX.Stop();
    //     }
    // }
}