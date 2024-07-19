using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class LaunchMeter : Meter
{
    [SerializeField] private float sliderSpeed;
    private float t; // used for ping pong of the slider

    protected void Update()
    {
        if (isEnabled)
        {
            t += sliderSpeed * Time.deltaTime;
            slider.value = Mathf.PingPong(t, slider.maxValue);
        }
    }

/*    public void CalculateForce(MeterData meterData)
    {
        float sliderValue = slider.value;
        float difference = meterData.maxValue - meterData.minValue;

        float diffPercent = sliderValue * difference;
        meterData.meterValue = diffPercent + meterData.minValue;
    }*/
}