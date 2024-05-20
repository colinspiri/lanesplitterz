using System;
using UnityEngine;

public class Pin : MonoBehaviour {
    private bool _hit;
    private bool _knockedDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_hit) {
            var zAngle = transform.localEulerAngles.z;
            if (zAngle > 5 || zAngle < 355) {
                _knockedDown = true;
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball") || collision.collider.CompareTag("Pin")) {
            _hit = true;
        }
    }
}
