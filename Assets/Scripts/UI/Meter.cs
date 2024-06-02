using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Meter : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] private GameObject meterUI;

    protected bool isEnabled;

    public void Start()
    {
        DisableMeter();
    }

    public void EnableMeter()
    {
        meterUI.SetActive(true);
        isEnabled = true;
    }
    public void DisableMeter()
    {
        meterUI.SetActive(false);
        isEnabled = false;
    }
}