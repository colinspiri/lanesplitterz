using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Yarn.Unity;
using UnityEngine.UI;
using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject playerArt;
    [SerializeField] private GameObject enemyArt;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject doublePointsUI;

    [Header("Scriptable Objects")]
    [SerializeField] private GameState gameState;
    [SerializeField] private UIConstants uiConstants;

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

    [YarnCommand("FadeInPlayerArt")]
    public void FadeInPlayerArt()
    {
        Color tempColor = playerArt.GetComponent<Image>().color;
        tempColor.a = 1f;
        playerArt.GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeOutPlayerArt")]
    public void FadeOutPlayerArt()
    {
        Color tempColor = playerArt.GetComponent<Image>().color;
        tempColor.a = 0.1f;
        playerArt.GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeInEnemyArt")]
    public void FadeInEnemyArt()
    {
        Color tempColor = enemyArt.GetComponent<Image>().color;
        tempColor.a = 1f;
        enemyArt.GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeOutEnemyArt")]
    public void FadeOutEnemyArt()
    {
        Color tempColor = enemyArt.GetComponent<Image>().color;
        tempColor.a = 0.1f;
        enemyArt.GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("EnableGameOverUI")]
    public void EnableGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    [YarnCommand("EnableScoreboard")]
    public void EnableScoreboard()
    {
        gameState.isScoreboardEnabled = true;
    }

    [YarnCommand("EnableDoublePointsUI")]
    public IEnumerator EnableDoublePointsUI()
    {
        doublePointsUI.SetActive(true);
        yield return new WaitForSeconds(uiConstants.doublePointsUITime);
        doublePointsUI.SetActive(false);
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

    [YarnCommand("EnableDialogue")]
    public void EnableDialogue()
    {
        DisableHUD();
        EnableBlur();
        EnablePlayerArt();
        EnableEnemyArt();
        EnableMouse();
    }

    // only call after art is enabled
    [YarnCommand("EnablePlayerDialogueElvis")]
    public void EnablePlayerDialogueElvis()
    {
        FadeInPlayerArt();
        FadeOutEnemyArt();
    }

    // only call after art is enabled
    [YarnCommand("EnableElvisDialogue")]
    public void EnableElvisDialogue()
    {
        FadeInEnemyArt();
        FadeOutPlayerArt();
    }

    [YarnCommand("DisableDialogue")]
    public void DisableDialogue()
    {
        EnableHUD();
        DisableBlur();
        DisablePlayerArt();
        DisableEnemyArt();
        DisableMouse();
    }
}
