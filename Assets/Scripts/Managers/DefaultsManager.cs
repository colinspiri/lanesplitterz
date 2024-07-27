using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class DefaultsManager : MonoBehaviour
{
    private static DefaultsManager instance;

    [Header("Audio Defaults")]
    [SerializeField] FloatVariable volumeSliderValue;
    [SerializeField] float defaultVolumeSliderValue;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        volumeSliderValue.Value = defaultVolumeSliderValue;
    }
}
