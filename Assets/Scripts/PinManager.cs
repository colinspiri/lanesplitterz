using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class PinManager : MonoBehaviour {
    public static PinManager Instance;
    
    [SerializeField] private IntVariable pinsKnockedDown;
    [SerializeField] private IntVariable currentPoints;

    public static Action OnPinKnockedDown;
    public static Action OnPinHitByBall;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pinsKnockedDown.Value = 0;
    }

    // called by Pins to tell PinManager that it's been knocked down. PinManager handles the counting and points
    public void NotifyPinKnockedDown(Pin pin) {
        pinsKnockedDown.Value++;
        currentPoints.Value += pin.PointValue;
        
        OnPinKnockedDown?.Invoke();
    }

    public void NotifyPinHitByBall(Pin pin)
    {
        OnPinHitByBall?.Invoke();
    }
}