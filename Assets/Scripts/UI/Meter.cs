using DG.Tweening;
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
        meterUI.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        isEnabled = true;
    }
    public virtual void DisableMeter()
    {
        meterUI.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.1f);
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