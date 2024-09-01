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
    [SerializeField] AudioClipRandomizer TypingVO;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] GameState gameState;
    public int frequency = 2;
    private bool swing = false;
    private int stop;
    private string unknownCharacter;

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
        else if (characterName.text == "Emiliana" || characterName.text == "Emi")
        {
            if (stop == 1)
            {
                MentorVO.PlaySFX();
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        else if (characterName.text == "Kaiba")
        {
            if (stop == 1)
            {
                CorpoVO.PlaySFX();
                stop = frequency;
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
                    stop = frequency * 2;
                }
                else
                {
                    stop = frequency;
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
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        else if (characterName.text == "Caesar")
        {
            if (stop == 1)
            {
                CaesarVO.PlaySFX();
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        else if (characterName.text == "")
        {

        }
        else if (unknownCharacter == "Corpo")
        {
            if (stop == 1)
            {
                CorpoVO.PlaySFX();
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        else if (unknownCharacter == "Elvis")
        {
            if (stop == 1)
            {
                ElvisVO.PlaySFX();
                if (swing == false)
                {
                    stop = frequency * 2;
                }
                else
                {
                    stop = frequency;
                }
                swing = !swing;
            }
            else
            {
                stop--;
            }
        }
        else if (unknownCharacter == "HiroandE1")
        {
            if (stop == 1)
            {
                ProtagVO.PlaySFX();
                ElvisVO.PlaySFX();
                stop = frequency;
            }
            else
            {
                stop--;
            }
        }
        
    }

    [YarnCommand("SetVO")]
    public void SetVO(string character)
    {
        unknownCharacter = character;
    }

}
