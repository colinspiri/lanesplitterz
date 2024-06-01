using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public TextMeshProUGUI _speedometer;
    
    private Rigidbody _playerBall;
    private float _currentSpeed = 0f;
    
    void Awake()
    {
        _speedometer.text = "Speed: " + _currentSpeed.ToString("F3");
        _playerBall = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        _currentSpeed = _playerBall.velocity.magnitude;
        _speedometer.text = "Speed: " + _currentSpeed.ToString("F3");
    }
}
