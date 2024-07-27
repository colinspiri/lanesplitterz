using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjectArchitecture;

public class MasterVolumeSlider : MonoBehaviour
{
    [SerializeField] MasterVolume masterVolume;
    [SerializeField] FloatVariable volumeSliderValue;

    void Start()
    {
        gameObject.GetComponent<Slider>().value = volumeSliderValue.Value;
    }
    public void SetMasterVolume(float volume)
    {
        masterVolume.SetMasterVolume(volume);
    }
}
