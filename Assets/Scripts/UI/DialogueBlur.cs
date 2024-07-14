using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Yarn.Unity;

public class DialogueBlur : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject playerArt;
    [SerializeField] private GameObject enemyArt;
    [SerializeField] private GameObject gameOverUI;
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

    [YarnCommand("EnablePlayerArt")]
    public void EnablePlayerArt()
    {
        playerArt.SetActive(true);
    }

    [YarnCommand("DisablePlayerArt")]
    public void DisablePlayerArt()
    {
        playerArt.SetActive(false);
    }

    [YarnCommand("EnableEnemyArt")]
    public void EnableEnemyArt()
    {
        enemyArt.SetActive(true);
    }

    [YarnCommand("DisableEnemyArt")]
    public void DisableEnemyArt()
    {
        enemyArt.SetActive(false);
    }

    [YarnCommand("EnableGameOverUI")]
    public void EnableGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    [YarnCommand("EnableMouse")]
    public void EnableMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    [YarnCommand("DisableMouse")]
    public void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
