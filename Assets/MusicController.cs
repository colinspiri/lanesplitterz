using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] AdaptiveMusicContainer tutorialMusic;
    [SerializeField] AdaptiveMusicContainer[] gameMusic;
    private bool launched = false;
    private int currentLevel = 0;

    private void Start()
    {
        tutorialMusic.RunContainer();
    }

    public void Launch()
    {
        if (!launched && currentLevel > 0)
        {
            gameMusic[currentLevel-1].TransitionSection(0);
            launched = true;
        }
        
    }

    public void NextLevel(int level)
    {
        if (level <= gameMusic.Length) {
            if (level == 1)
            {
                tutorialMusic.SetState(1);
            }
            else
            {
                gameMusic[currentLevel - 1].SetState(1);
            }
            currentLevel = level;
            gameMusic[currentLevel - 1].RunContainer();
        }
        
        
    }
}
