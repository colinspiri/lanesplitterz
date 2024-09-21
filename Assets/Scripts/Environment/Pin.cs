using System;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Serialization;

public class Pin : MonoBehaviour {
    // components
    [Tooltip("Pin type to instantiate when this pin is respawned.")] 
    [SerializeField] public PinType pinType;
    
    // public data
    [Tooltip("Range of angles that pin is considered upright. When euler angle is > the range or < (360 - the range), the pin is considered knocked over. In degrees.")]
    [SerializeField] private float uprightAngleRange = 5;
    [SerializeField] private int pointValue = 1;
    public int PointValue => pointValue;
    public PinCluster parentCluster = null;
    
    // private state
    public enum PinState { Untouched, Hit, KnockedDown }
    [FormerlySerializedAs("_pinState")] public PinState pinState;
    public enum LastTouched { None, PlayerBall, EnemyBall }
    public LastTouched LastTouchedBy { get; private set; }

    // misc
    private int _ballLayer;
    private int _pinLayer;

    void Start()
    {
        PinManager.Instance.AddPin(this);

        _ballLayer = LayerMask.NameToLayer("Balls");
        _pinLayer = gameObject.layer;
    }

    void Update()
    {
        // after being hit, start checking if the angle exceeds a certain range
        if (pinState == PinState.Hit) {
            var zAngle = transform.localEulerAngles.z;
            if (zAngle > uprightAngleRange || zAngle < (360 - uprightAngleRange)) {
                SetKnockedDown();
            }
        }
    }

    private void SetKnockedDown() {
        pinState = PinState.KnockedDown;

        PinManager.Instance.NotifyPinKnockedDown(this);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = collision.gameObject.layer;
        
        // set last touched by
        if (pinState != PinState.KnockedDown) {
            if (collisionLayer == _pinLayer) {
                var pin = collision.gameObject.GetComponent<Pin>();
                if (pin) LastTouchedBy = pin.LastTouchedBy;
            }
            else if (collision.gameObject.CompareTag("Player")) {
                LastTouchedBy = LastTouched.PlayerBall;
            }
            else if (collision.gameObject.CompareTag("Enemy Ball")) {
                LastTouchedBy = LastTouched.EnemyBall;
            }
        }

        // set state to hit
        if (pinState == PinState.Untouched &&
            (collisionLayer == _pinLayer || collisionLayer == _ballLayer))
        {
            pinState = PinState.Hit;
            if (collisionLayer == _ballLayer)
            {
                PinManager.Instance.NotifyPinHitByBall(this);
            }
        }
    }

    private void OnDestroy() {
        PinManager.Instance.DestroyPin(this);
    }
}
