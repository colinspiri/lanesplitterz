using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] public float currentSpeed = 0f;
    [SerializeField] private Rigidbody ball;
    [SerializeField] public TextMeshProUGUI _speedometer;

    // Start is called before the first frame update
    void Start()
    {
        _speedometer.text = "Speed: " + currentSpeed.ToString("F3");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentSpeed = ball.velocity.magnitude;
        _speedometer.text = "Speed: " + currentSpeed.ToString("F3");

    }
}
