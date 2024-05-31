using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PowerLevelManager : MonoBehaviour
{
    public static PowerLevelManager Instance;

    [SerializeField] private GameObject powerBarUI;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private float powerBarSpeed;

    private bool powerSliderEnabled;
    private float t; // used for ping pong of the slider

    private void Awake()
    {
        Instance = this;
        powerSliderEnabled = false;
        powerBarUI.gameObject.SetActive(false);
        t = 0;
    }

    private void Update()
    {
        if (powerSliderEnabled)
        {
            t += powerBarSpeed * Time.deltaTime;
            powerSlider.value = Mathf.PingPong(t, powerSlider.maxValue);
        }
    }

    public float CalculateLaunchForce(float minForce, float maxForce)
    {
        float powerSliderValue = powerSlider.value;
        float difference = maxForce - minForce;

        float diffPercent = powerSliderValue * difference;
        float launchForce = diffPercent + minForce;

        return launchForce;
    }

    public void EnablePowerSlider()
    {
        powerBarUI.gameObject.SetActive(true);
        powerSliderEnabled = true;
        t = 0;
    }

    public void DisablePowerSlider()
    {
        powerBarUI.gameObject.SetActive(false);
        powerSliderEnabled = false;
    }
}
