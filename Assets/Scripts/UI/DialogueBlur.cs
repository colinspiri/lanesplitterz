using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Yarn.Unity;

public class DialogueBlur : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject blur;
    private Transform _myCam;

    void Start()
    {
        _myCam = GameObject.FindWithTag("MainCamera").transform;
    }

    [YarnCommand("DisableHud")]
    public void DisableHUD()
    {
        hud.SetActive(false);
    }

    [YarnCommand("EnableHud")]
    public void EnableHUD()
    {
        hud.SetActive(true);
    }

    [YarnCommand("EnableBlur")]
    public void EnableBlur()
    {
        blur.gameObject.SetActive(true);
    }

    [YarnCommand("DisableBlur")]
    public void DisableBlur()
    {
        blur.gameObject.SetActive(false);
    }
}
