using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MusicController : MonoBehaviour
{
    [SerializeField] AdaptiveMusicContainer tutorialMusic;
    [SerializeField] AdaptiveMusicContainer[] gameMusic;
    [SerializeField] GameState gameState;
    [SerializeField] DialogueManager dialogueManager;
    private AdaptiveMusicContainer currentMusic;
    private bool launched = false;

    private void Start()
    {
        if (gameState.currentLevelIndex == 0)
        {
            currentMusic = tutorialMusic;
            currentMusic.RunContainer();
        }
        else
        {
            int newLevel = (gameState.currentLevelIndex - 1) / 5;
            currentMusic = gameMusic[newLevel];
            currentMusic.RunContainer();
        }
        
    }

    public void Launch()
    {
        if (!launched && gameState.currentLevelIndex > 0)
        {
            currentMusic.TransitionSection(0);
            launched = true;
        }
        
    }

    public void NextLevel()
    {
        if (gameState.currentLevelIndex % 5 == 1) {
            if (gameState.currentLevelIndex == 1)
            {
                currentMusic.SetState(1);
            }
            int newLevel = (gameState.currentLevelIndex - 1) / 5;
            currentMusic = gameMusic[newLevel];
            currentMusic.RunContainer();
            dialogueManager.SetNewGameMusic(currentMusic);
            launched = false;
        }
    }

    [YarnCommand("BeginIntermission")]
    public void BeginIntermission()
    {
        currentMusic.TransitionSection(2);
    }

    [YarnCommand("EndIntermission")]
    public void EndIntermission()
    {
        currentMusic.TransitionSection(0);
    }
}
