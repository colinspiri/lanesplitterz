using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Meter : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] private GameObject meterUI;

    protected bool isEnabled;

    public virtual void Start()
    {
        DisableMeter();
    }

    public virtual void EnableMeter()
    {
        meterUI.SetActive(true);
        isEnabled = true;
    }
    public virtual void DisableMeter()
    {
        meterUI.SetActive(false);
        isEnabled = false;
    }

    public void CalculateForce(MeterData meterData)
    {
        float sliderValue = slider.value;
        float difference = meterData.maxValue - meterData.minValue;

        float diffPercent = sliderValue * difference;
        meterData.meterValue = diffPercent + meterData.minValue;
    }
}