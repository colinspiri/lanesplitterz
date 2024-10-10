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
    [SerializeField] private GameObject playAgainUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private Image fadeToBlackUI;
    [SerializeField] private RectTransform checkerboard;
    [SerializeField] private GameObject replayTutorialButton;
    [SerializeField] private GameObject playGameButton;

    [Header("Variables")] 
    [SerializeField] private float checkerboardOffscreenDistance;
    [SerializeField] private float checkerboardSlideTime;
    
    [Header("Scriptable Objects")]
    [SerializeField] private GameState gameState;
    [SerializeField] private UIConstants uiConstants;
    [SerializeField] private PlayerInfo playerInfo;

    [Header("Game Events")]
    [SerializeField] private GameEvent endIntermission;
    [SerializeField] private GameEvent startTabTutorialDialogue;

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
        // player beat the game
        if (gameState.currentLevelIndex >= 15 && CheckIsPlayerWinning())
        {
            gameState.isPauseMenuEnabled = false;
            fadeToBlackUI.DOFade(1f, 2f)
            .OnComplete(() =>
            {
                // Load credits scene
                SceneLoaderManager.Instance.LoadCreditsScene();
            });
        }
        // player beat either elvis or corpo
        else if (CheckIsPlayerWinning())
        {
            gameState.isPauseMenuEnabled = false;
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
            playAgainUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        ScoreboardUI.Instance.HideFinalScores();
        ScoreboardUI.Instance.ClearScoreboard();
    }

    // set playAgainUI to inactive when player clicks play again after losing
    public void SetUIToInactive(GameObject gameObject) => gameObject.SetActive(false);

    public void StartNewRound() => RoundManager.OnNewRound.Invoke();

    [YarnCommand("EnableScoreboard")]
    public void EnableScoreboard()
    {
        //gameState.isScoreboardEnabled = true;
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
        checkerboard.gameObject.SetActive(true);
        checkerboard.anchoredPosition = new Vector2(checkerboardOffscreenDistance, 0);
        checkerboard.DOAnchorPosX(0f, checkerboardSlideTime).SetUpdate(true);
    }

    [YarnCommand("DisableCheckerboard")]
    public void DisableCheckerboard() {
        checkerboard.anchoredPosition = new Vector2(0, 0);
        checkerboard.DOAnchorPosX(checkerboardOffscreenDistance, checkerboardSlideTime).SetUpdate(true).onComplete += () =>
        {
            checkerboard.gameObject.SetActive(false);
            EndIntermission();
        };
    }

    [YarnCommand("CompleteIntermission")]
    public void EndIntermission() => endIntermission.Raise();

    [YarnCommand("StartTabTutorial")]
    public void StartTabTutorial() => StartCoroutine(TabTutorialCoroutine());

    private IEnumerator TabTutorialCoroutine()
    {
        gameState.isPauseMenuEnabled = false;
        yield return new WaitForSeconds(1f);
        gameState.isPauseMenuEnabled = true;
        startTabTutorialDialogue.Raise();
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
        art.transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.1f).SetUpdate(true);
    }

    [YarnCommand("EnableArt")]
    public void EnableArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        Color tempColor2 = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 0.25f;
        tempColor2.g = 0.25f;
        tempColor2.b = 0.25f;
        artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor2;

        artOne.transform.GetChild(0).gameObject.SetActive(true);
        artTwo.transform.GetChild(0).gameObject.SetActive(true);
        artOne.transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.1f).SetUpdate(true);
        artTwo.transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.1f).SetUpdate(true);
    }

    [YarnCommand("DisableArt")]
    public void DisableArt(GameObject artOne, GameObject artTwo)
    {
        DisableOneArt(artOne);
        DisableOneArt(artTwo);

        //artOne.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.1f).SetUpdate(true);
        //artTwo.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.1f).SetUpdate(true);
        //artOne.transform.GetChild(0).gameObject.SetActive(false);
        //artTwo.transform.GetChild(0).gameObject.SetActive(false);
    }

    [YarnCommand("DisableOneArt")]
    public void DisableOneArt(GameObject art)
    {
        art.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.1f).SetUpdate(true)
            .onComplete += () => 
            { art.transform.GetChild(0).gameObject.SetActive(false); };
    }

    /// <summary>
    /// Enables one art and disables another while also darkening the art of the other character
    /// </summary>
    /// <param name="artOne">Art to enable</param>
    /// <param name="artTwo">Art to disable</param>
    /// <param name="artThree">Art to darken</param>
    [YarnCommand("EnableDisableArt")]
    public void EnableDisableArt(GameObject artOne, GameObject artTwo, GameObject artThree)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        Color tempColor2 = artThree.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 0.25f;
        tempColor2.g = 0.25f;
        tempColor2.b = 0.25f;
        artThree.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor2, 0.1f);

        EnableOneArt(artOne);
        DisableOneArt(artTwo);
    }

    [YarnCommand("EnableDisableArtTutorial")]
    public void EnableDisableArtTutorial(GameObject artOne, GameObject artTwo)
    {
        EnableOneArt(artOne);
        DisableOneArt(artTwo);
    }

    [YarnCommand("EnableFadeArt")]
    public void EnableFadeArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        Color tempColor2 = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 0.25f;
        tempColor2.g = 0.25f;
        tempColor2.b = 0.25f;
        artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor2;

        EnableOneArt(artOne);
        EnableOneArt(artTwo);

        // artTwo should already have color immediately set without tweening
        //FadeArt(artOne, artTwo);
    }

    [YarnCommand("FadeArt")]
    public void FadeArt(GameObject fadeIn, GameObject fadeOut)
    {
        Color tempColor = fadeIn.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        //fadeIn.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        fadeIn.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor, 0.1f).SetUpdate(true);

        Color tempColor2 = fadeOut.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 0.25f;
        tempColor2.g = 0.25f;
        tempColor2.b = 0.25f;
        //fadeOut.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        fadeOut.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor2, 0.1f).SetUpdate(true);
    }

    [YarnCommand("FadeOutBothArt")]
    public void FadeOutBothArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 0.25f;
        tempColor.g = 0.25f;
        tempColor.b = 0.25f;
        //artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        artOne.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor, 0.1f).SetUpdate(true);

        Color tempColor2 = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 0.25f;
        tempColor2.g = 0.25f;
        tempColor2.b = 0.25f;
        //artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        artTwo.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor2, 0.1f).SetUpdate(true);
    }

    [YarnCommand("FadeInBothArt")]
    public void FadeInBothArt(GameObject artOne, GameObject artTwo)
    {
        Color tempColor = artOne.transform.GetChild(0).GetComponent<Image>().color;
        tempColor.r = 1f;
        tempColor.g = 1f;
        tempColor.b = 1f;
        //artOne.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        artOne.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor, 0.1f).SetUpdate(true);

        Color tempColor2 = artTwo.transform.GetChild(0).GetComponent<Image>().color;
        tempColor2.r = 1f;
        tempColor2.g = 1f;
        tempColor2.b = 1f;
        //artTwo.transform.GetChild(0).GetComponent<Image>().color = tempColor;

        artTwo.transform.GetChild(0).GetComponent<Image>().DOColor(tempColor2, 0.1f).SetUpdate(true);
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

    [YarnCommand("DisplayTutorialButtons")]
    public void DisplayTutorialButtons()
    {
        EnableMouse();
        replayTutorialButton.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        playGameButton.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
    }

    [YarnCommand("SetPlayerReady")]
    public void SetPlayerReady() => playerInfo.isReady = true;

    #endregion
}