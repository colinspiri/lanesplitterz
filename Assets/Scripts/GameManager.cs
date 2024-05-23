using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [SerializeField] private IntVariable currentPoints;
    
    private bool _paused;
    public bool GamePaused => _paused;

    private float _timeScaleBeforePause = 1f;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        currentPoints.Value = 0;
    }
    
    public void Pause(bool pauseAudio = true) {
        _paused = true;

        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;

        if(pauseAudio) AudioListener.pause = true;
    }

    public void Resume(bool resumeAudio = true) {
        _paused = false;
        Time.timeScale = _timeScaleBeforePause;
        
        if(resumeAudio) AudioListener.pause = false;
    }
}