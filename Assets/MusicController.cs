using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] AdaptiveMusicContainer gameMusic;
    private bool launched = false;

    private void Start()
    {
        gameMusic.RunContainer();
    }

    public void Launch()
    {
        if (!launched)
        {
            gameMusic.TransitionSection(0);
            launched = true;
        }
        
    }
}
