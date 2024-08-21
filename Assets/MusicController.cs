using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] AdaptiveMusicContainer tutorialMusic;
    [SerializeField] AdaptiveMusicContainer[] gameMusic;
    [SerializeField] GameState gameState;
    [SerializeField] DialogueManager dialogueManager;
    private bool launched = false;

    private void Start()
    {
        if (gameState.currentLevelIndex == 0)
        {
            tutorialMusic.RunContainer();
        }
        else
        {
            int newLevel = (gameState.currentLevelIndex - 1) / 5;
            gameMusic[newLevel].RunContainer();
        }
        
    }

    public void Launch()
    {
        if (!launched && gameState.currentLevelIndex > 0)
        {
            gameMusic[(gameState.currentLevelIndex - 1) / 5].TransitionSection(0);
            launched = true;
        }
        
    }

    public void NextLevel()
    {
        if (gameState.currentLevelIndex % 5 == 1) {
            if (gameState.currentLevelIndex == 1)
            {
                tutorialMusic.SetState(1);
            }
            int newLevel = (gameState.currentLevelIndex - 1) / 5;
            gameMusic[newLevel].RunContainer();
            dialogueManager.SetNewGameMusic(gameMusic[newLevel]);
            launched = false;
        }
    }
}
