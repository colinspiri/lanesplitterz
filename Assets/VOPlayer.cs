using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class VOPlayer : MonoBehaviour
{
    [SerializeField] AudioClipRandomizer ProtagVO;
    [SerializeField] AudioClipRandomizer MentorVO;
    [SerializeField] AudioClipRandomizer ElvisVO;
    [SerializeField] AudioClipRandomizer CorpoVO;
    [SerializeField] AudioClipRandomizer CaesarVO;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] GameState gameState;
    public int frequency = 2;
    private bool swing = false;
    private int stop;

    private void Start()
    {
        stop = frequency;
    }
    public void PlayCharacterVO()
    {
        if (characterName.text == "Hiro")
        {
            if (stop == 1)
            {
                ProtagVO.PlaySFX();
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        else if (characterName.text == "Emiliana")
        {
            if (stop == 1)
            {
                MentorVO.PlaySFX();
                stop = frequency * 2;
            }
            else
            {
                stop--;
            }
        }
        else if (gameState.currentLevelIndex < 6 || characterName.text == "E1")
        {
            if (stop == 1)
            {
                ElvisVO.PlaySFX();
                if (swing == false)
                {
                    stop = frequency * 4;
                }
                else
                {
                    stop = frequency * 2;
                }
                swing = !swing;
            }
            else
            {
                stop--;
            }
        }
        else if (gameState.currentLevelIndex < 11)
        {
            if (stop == 1)
            {
                CorpoVO.PlaySFX();
                stop = frequency * 2;
            }
            else
            {
                stop--;
            }
        }
        else
        {
            if (stop == 1)
            {
                CaesarVO.PlaySFX();
                stop = frequency * 2;
            }
            else
            {
                stop--;
            }
        }
        
    }
}
