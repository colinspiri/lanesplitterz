using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Yarn.Unity;
using UnityEngine.UI;
using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using DG.Tweening;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private Image fadeToBlackUI;
    [SerializeField] private GameObject doublePointsUI;
    [SerializeField] private GameObject checkerboard;

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
        //StartCoroutine(EnableGameOverUICoroutine());
        EnableGameOverUICoroutine();
    }

    public void EnableGameOverUICoroutine()
    {
        // player beat the game
        if (gameState.currentLevelIndex >= 15 && CheckIsPlayerWinning())
        {
            endGameUI.SetActive(true);
            EnableMouse();
            return;
            //yield break;
        }
        // player beat either elvis or corpo
        else if (CheckIsPlayerWinning())
        {
            // should take one second to fade in, wait one second, and take two seconds to fade out
/*            fadeToBlackUI.DOFade(1f, 2f).OnComplete(() => StartNewRound()).OnComplete(() => fadeToBlackUI.DOFade(1f, 1f))
                .OnComplete(() => fadeToBlackUI.DOFade(0f, 2f));*/

            fadeToBlackUI.DOFade(1f, 2f)
            .OnComplete(() =>
            {
                StartNewRound();
                fadeToBlackUI.DOFade(1f, 1f)
                    .OnComplete(() => fadeToBlackUI.DOFade(0f, 2f));
            });
        }
        // player lost to current boss
        else
        {
            DisableDialogueWithMouse();
            gameOverUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
/*            ScoreboardUI.Instance.HideFinalScores();
            ScoreboardUI.Instance.ClearScoreboard();
            ScoreboardUI.Instance.UpdateEnemyTitleText();
            yield break;*/
        }
/*        yield return new WaitForSeconds(2f);
        gameOverUI.SetActive(false);*/
        //StartNewRound();
        ScoreboardUI.Instance.HideFinalScores();
        ScoreboardUI.Instance.ClearScoreboard();
        ScoreboardUI.Instance.UpdateEnemyTitleText();
    }

    // set gameOverUI to inactive when player clicks play again after losing
    public void SetUIToInactive(GameObject gameObject) => gameObject.SetActive(false);

    public void StartNewRound() => RoundManager.OnNewRound.Invoke();

    [YarnCommand("EnableScoreboard")]
    public void EnableScoreboard()
    {
        //gameState.isScoreboardEnabled = true;
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

    [YarnFunction("CheckIsPlayerWinning")]
    public static bool CheckIsPlayerWinning()
    {
        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore) return true;
        return false;
    }

    [YarnCommand("EnableCheckerboard")]
    public void EnableCheckerboard()
    {
        checkerboard.SetActive(true);
    }

    [YarnCommand("DisableCheckerboard")]
    public void DisableCheckerboard()
    {
        checkerboard.SetActive(false);
    }

    #endregion

    #region Character Art Functions

    [YarnCommand("EnableDialogue")]
    public void EnableDialogue()
    {
        EnableBlur();
        DisableHUD();
        EnableMouse();
        gameState.isDialogueRunning = true;
    }

    [YarnCommand("EnableOneArt")]
    public void EnableOneArt(GameObject art)
    {
        art.transform.GetChild(0).gameObject.SetActive(true);
    }

    [YarnCommand("DisableArt")]
    public void DisableArt(GameObject artOne, GameObject artTwo)
    {
        artOne.transform.GetChild(0).gameObject.SetActive(false);
        artTwo.transform.GetChild(0).gameObject.SetActive(false);
    }

    [YarnCommand("DisableOneArt")]
    public void DisableOneArt(GameObject art)
    {
        art.transform.GetChild(0).gameObject.SetActive(false);
    }

    [YarnCommand("EnableFadeArt")]
    public void EnableFadeArt(GameObject artOne, GameObject artTwo)
    {
        EnableOneArt(artOne);
        EnableOneArt(artTwo);
        FadeArt(artOne, artTwo);
    }

    [YarnCommand("FadeArt")]
    public void FadeArt(GameObject fadeIn, GameObject fadeOut)
    {
        Color tempColor = fadeIn.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        fadeIn.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        tempColor = fadeOut.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 0.25f;
        tempColor.g = 0.25f;
        tempColor.b = 0.25f;
        fadeOut.transform.GetChild(0).GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeOutBothArt")]
    public void FadeOutBothArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 0.25f;
        tempColor.g = 0.25f;
        tempColor.b = 0.25f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        tempColor = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 0.25f;
        tempColor.g = 0.25f;
        tempColor.b = 0.25f;
        artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("FadeInBothArt")]
    public void FadeInBothArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        tempColor = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor;
    }

    [YarnCommand("DisableDialogue")]
    public void DisableDialogue()
    {
        EnableHUD();
        DisableBlur();
        DisableMouse();
        gameState.isDialogueRunning = false;
    }

    [YarnCommand("DisableDialogueWithMouse")]
    public void DisableDialogueWithMouse()
    {
        EnableHUD();
        DisableBlur();
        gameState.isDialogueRunning = false;
    }

    #endregion
}