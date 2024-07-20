using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class VOPlayer : MonoBehaviour
{
    [SerializeField] AudioClipRandomizer ProtagVO;
    [SerializeField] AudioClipRandomizer ElvisVO;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] string elvisName;
    public int frequency = 2;
    private bool swing = false;
    private int stop;

    private void Start()
    {
        stop = frequency;
    }
    public void PlayCharacterVO()
    {
        if (characterName.text == elvisName)
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
}
