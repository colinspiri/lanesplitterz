using System;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;
    
    private float _fixedDeltaTime;
    private float _previousTimeScale = 1;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        this._fixedDeltaTime = Time.fixedDeltaTime;
    }

    public void SetTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = this._fixedDeltaTime * Time.timeScale;
    }

    public void PauseTime() {
        _previousTimeScale = Time.timeScale;
        SetTimeScale(0);
    }
    public void ResumeTime() {
        SetTimeScale(_previousTimeScale);
    }

    private void OnDestroy() {
        SetTimeScale(1);
    }
}