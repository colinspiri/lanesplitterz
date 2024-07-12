using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOPlayer : MonoBehaviour
{
    [SerializeField] AudioClipRandomizer ProtagVO;
    public int frequency = 2;
    private int stop;

    private void Start()
    {
        stop = frequency;
    }
    public void PlayCharacterVO()
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
}
