using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayWhack : MonoBehaviour
{
    [SerializeField] AudioSource WhackSFX;

    [YarnCommand("PlayWhack")]
    public void PlayWhackSFX()
    {
        WhackSFX.Play();
    }
}
