using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [SerializeField] AudioSource pauseMusic;
    
    private bool _paused;
    public bool GamePaused => _paused;
    
    private void Awake() {
        Instance = this;
        pauseMusic.ignoreListenerPause = true;
    }

    public void Pause(bool pauseAudio = true) {
        _paused = true;

        TimeManager.Instance.PauseTime();

        if (pauseAudio)
        {
            AudioListener.pause = true;
            pauseMusic.Play();
        }

    }

    public void Resume(bool resumeAudio = true) {
        _paused = false;
        
        TimeManager.Instance.ResumeTime();

        if (resumeAudio)
        {
            AudioListener.pause = false;
            pauseMusic.Stop();
        }
    }
}