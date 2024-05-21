using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class Pin : MonoBehaviour {
    // components
    [SerializeField] private PinCollection allPins;
    [SerializeField] private IntVariable currentPoints;

    [SerializeField] private int pointValue = 1;
    
    [Tooltip("Range of angles that pin is considered upright. When euler angle is > the range or < (360 - the range), the pin is considered knocked over. In degrees.")]
    [SerializeField] private float uprightAngleRange = 5;
    
    private bool _hit;
    private bool _knockedDown;

    // Start is called before the first frame update
    void Start()
    {
        allPins.Add( this);
    }

    // Update is called once per frame
    void Update()
    {
        // after being hit, start checking if the angle exceeds a certain range
        if (_hit && !_knockedDown) {
            var zAngle = transform.localEulerAngles.z;
            if (zAngle > uprightAngleRange || zAngle < (360 - uprightAngleRange)) {
                SetKnockedDown();
            }
        }
    }

    private void SetKnockedDown() {
        _knockedDown = true;
        currentPoints.Value += pointValue;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball") || collision.collider.CompareTag("Pin")) {
            _hit = true;
        }
    }

    private void OnDestroy() {
        allPins.Remove(this);
    }
}
