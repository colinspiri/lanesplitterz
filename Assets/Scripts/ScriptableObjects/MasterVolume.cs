using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using ScriptableObjectArchitecture;

[CreateAssetMenu(fileName = "MasterVolume", menuName = "MasterVolume")]

public class MasterVolume : ScriptableObject
{
    public FloatVariable volumeSliderValue;
    public AudioMixer audioMixer;
    private float masterVolume;

    public void SetMasterVolume(float volume)
    {
        float logCalculation = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("Volume", logCalculation);
        volumeSliderValue.Value = volume;
    }

    public float GetMasterVolume()
    {
        audioMixer.GetFloat("Volume", out masterVolume);
        return masterVolume;
    }
}
