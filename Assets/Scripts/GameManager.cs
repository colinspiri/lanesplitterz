using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [SerializeField] private IntVariable currentPoints;
    
    private bool _paused;
    public bool GamePaused => _paused;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        currentPoints.Value = 0;
    }
    
    public void Pause(bool pauseAudio = true) {
        _paused = true;

        TimeManager.Instance.PauseTime();

        if(pauseAudio) AudioListener.pause = true;
    }

    public void Resume(bool resumeAudio = true) {
        _paused = false;
        
        TimeManager.Instance.ResumeTime();
        
        if(resumeAudio) AudioListener.pause = false;
    }
}