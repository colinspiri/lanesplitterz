using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    public bool pausedForTutorial;

    private Stack<float> _timeScales;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _timeScales = new Stack<float>();
    }

    private void SetTimeScaleUnstored(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    // sets timescale while ignoring tutorial pauses - use carefully!
    private void SetTimeScaleUnsafe(float newTimeScale)
    {
        _timeScales.Push(Time.timeScale);
        SetTimeScaleUnstored(newTimeScale);
    }

    public void SetTimeScale(float newTimeScale)
    {
        if (pausedForTutorial) return;
        
        _timeScales.Push(Time.timeScale);
        SetTimeScaleUnstored(newTimeScale);
    }

    public void PauseForTutorial()
    {
        if (pausedForTutorial) return;
        pausedForTutorial = true;
        PauseTime();
    }

    public void ResumeFromTutorialPause()
    {
        if (pausedForTutorial)
        {
            pausedForTutorial = false;
        }

        ResumeTime();
    }

    public void PauseTime()
    {
        SetTimeScaleUnsafe(0);
    }

    public void ResumeTime()
    {
        if (_timeScales.Count == 0)
        {
            SetTimeScaleUnstored(1);
            return;
        }
        float previousTimeScale = _timeScales.Pop();
        SetTimeScaleUnstored(previousTimeScale);
    }

    private void OnDestroy()
    {
        SetTimeScaleUnstored(1);
    }
}