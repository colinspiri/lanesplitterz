using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public TextMeshProUGUI _speedometer;
    private Rigidbody _playerBall;
    private Transform cameraPosition;
    private float _currentSpeed = 0f;
    
    void Awake()
    {
        _speedometer.text = "Speed: " + _currentSpeed.ToString("F2");
    }

    void Start()
    {
        cameraPosition = GameObject.FindWithTag("MainCamera").transform;

        _playerBall = PlayerMovement.Instance.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _currentSpeed = Vector3.Project( _playerBall.velocity, cameraPosition.forward ).magnitude;
        _speedometer.text = "Speed: " + _currentSpeed.ToString("F2");
        
        //Debug.Log("speed: " + _currentSpeed);
    }
}
