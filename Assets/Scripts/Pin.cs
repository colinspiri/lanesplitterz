using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class Pin : MonoBehaviour {
    // components
    [SerializeField] private PinCollection allPins;

    // public data
    [Tooltip("Range of angles that pin is considered upright. When euler angle is > the range or < (360 - the range), the pin is considered knocked over. In degrees.")]
    [SerializeField] private float uprightAngleRange = 5;
    [SerializeField] private int pointValue = 1;
    public int PointValue => pointValue;
    
    // private state
    private enum PinState { Untouched, Hit, KnockedDown }
    private PinState _pinState;

    void Start()
    {
        allPins.Add( this);
    }

    void Update()
    {
        // after being hit, start checking if the angle exceeds a certain range
        if (_pinState == PinState.Hit) {
            var zAngle = transform.localEulerAngles.z;
            if (zAngle > uprightAngleRange || zAngle < (360 - uprightAngleRange)) {
                SetKnockedDown();
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetKnockedDown() {
        _pinState = PinState.KnockedDown;

        PinManager.Instance.NotifyPinKnockedDown(this);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_pinState == PinState.Untouched && (collision.collider.CompareTag("Ball") || collision.collider.CompareTag("Pin"))) {
            _pinState = PinState.Hit;
            if (collision.collider.CompareTag("Ball"))
            {
                PinManager.Instance.NotifyPinHitByBall(this);
            }
        }
    }

    private void OnDestroy() {
        allPins.Remove(this);
    }
}
