using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class Pin : MonoBehaviour {
    // components
    [Tooltip("Pin type to instantiate when this pin is respawned.")] 
    [SerializeField] public PinType pinType;
    
    // public data
    [Tooltip("Range of angles that pin is considered upright. When euler angle is > the range or < (360 - the range), the pin is considered knocked over. In degrees.")]
    [SerializeField] private float uprightAngleRange = 5;
    [SerializeField] private int pointValue = 1;
    public int PointValue => pointValue;
    
    // private state
    private enum PinState { Untouched, Hit, KnockedDown }
    private PinState _pinState;
    public enum LastTouched { None, PlayerBall, EnemyBall }
    public LastTouched LastTouchedBy { get; private set; }

    // misc
    private int _ballLayer;

    void Start()
    {
        PinManager.Instance.AddPin(this);

        _ballLayer = LayerMask.NameToLayer("Balls");
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

    private void SetKnockedDown() {
        _pinState = PinState.KnockedDown;

        PinManager.Instance.NotifyPinKnockedDown(this);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = collision.gameObject.layer;
        
        // set last touched by
        if (_pinState != PinState.KnockedDown) {
            if (collision.gameObject.CompareTag("Pin")) {
                var pin = collision.gameObject.GetComponent<Pin>();
                LastTouchedBy = pin.LastTouchedBy;
            }
            else if (collision.gameObject.CompareTag("Player")) {
                LastTouchedBy = LastTouched.PlayerBall;
            }
            else if (collision.gameObject.CompareTag("Enemy Ball")) {
                LastTouchedBy = LastTouched.EnemyBall;
            }
        }

        // set state to hit
        if (_pinState == PinState.Untouched &&
            (collision.collider.CompareTag("Pin") || collisionLayer == _ballLayer))
        {
            _pinState = PinState.Hit;
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
