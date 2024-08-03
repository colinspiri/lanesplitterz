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
/*    [SerializeField] private GameObject playerArt;
    [SerializeField] private GameObject enemyArt;*/
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

    #region Utility Dialogue Functions

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

    #endregion

    #region Character Art Functions

    [YarnCommand("EnableDialogue")]
    public void EnableDialogue()
    {
        EnableBlur();
        DisableHUD();
        EnableMouse();
    }

    [YarnCommand("EnableArt")]
    public void EnableArt(GameObject art)
    {
        art.transform.GetChild(0).gameObject.SetActive(true);
    }

    [YarnCommand("DisableArt")]
    public void DisableArt(GameObject artOne, GameObject artTwo)
    {
        artOne.transform.GetChild(0).gameObject.SetActive(false);
        artTwo.transform.GetChild(0).gameObject.SetActive(false);
    }

    // used for emiliana
    [YarnCommand("EnableOneArt")]
    public void EnableOneArt(GameObject art)
    {
        art.SetActive(true);
    }

    // used for emiliana
    [YarnCommand("DisableOneArt")]
    public void DisableOneArt(GameObject art)
    {
        art.SetActive(false);
    }

    [YarnCommand("EnableFadeArt")]
    public void EnableFadeArt(GameObject artOne, GameObject artTwo)
    {
        EnableArt(artOne);
        EnableArt(artTwo);
        FadeArt(artOne, artTwo);
    }

    [YarnCommand("FadeArt")]
    public void FadeArt(GameObject fadeIn, GameObject fadeOut)
    {
        Color tempColor = fadeIn.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.a = 1f;
        fadeIn.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        tempColor = fadeOut.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.a = 0.1f;
        fadeOut.transform.GetChild(0).GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeOutBothArt")]
    public void FadeOutBothArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.a = 0.1f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        tempColor = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.a = 0.1f;
        artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("DisableDialogue")]
    public void DisableDialogue()
    {
        EnableHUD();
        DisableBlur();
        DisableMouse();
    }

    // EnableArt
    // DisableArt
    // FadeArt
    // EnableFadeArt

    #endregion
}