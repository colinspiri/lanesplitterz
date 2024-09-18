using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script is used instead of scene loader scriptable object because we need a coroutine to load the next level
// Should ONLY be used for loading levels, no other scenes including game scene
public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance;
    [SerializeField] MusicController musicController;
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameObject e1Cannon;
    [SerializeField] private GameObject kaibaCannon;
    [SerializeField] private GameObject caesarCannon;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RoundManager.OnNewRound += InitializeLevel;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    private void InitializeLevel()
    {
        if (playerInfo.isPracticing == true)
        {
            LoadTutorialLevel();
            return;
        }
        if (playerInfo.isPracticing == false && playerInfo.finishedTutorial == true)
        {
            sceneLoader.LoadNewLevel();
            playerInfo.finishedTutorial = false;
            return;
        }
        // if the player loses we want them to choose if they want to play again, not automatically load the next level
        if (gameState.currentLevelIndex % 5 == 0 && !playerInfo.isWinning) return;
        else sceneLoader.LoadNewLevel();
    }

    public void LoadTutorialLevel()
    {
        StartCoroutine(LoadTutorialLeveCoroutine());
    }

    public IEnumerator LoadTutorialLeveCoroutine()
    {
        yield return SceneManager.UnloadSceneAsync("Level 0");
        SceneManager.LoadScene("Level 0", LoadSceneMode.Additive);
    }

    public IEnumerator LoadLevelCoroutine()
    {
        yield return SceneManager.UnloadSceneAsync("Level " + gameState.currentLevelIndex.ToString());

        // replay boss if player lost
        if (playerInfo.isWinning == false && gameState.currentLevelIndex % 5 == 0 && gameState.currentLevelIndex > 0)
        {
            SceneManager.LoadScene("Level " + (gameState.currentLevelIndex - 4).ToString(), LoadSceneMode.Additive);
            gameState.currentLevelIndex -= 4;
            RoundManager.Instance.ClearFinalScores();
            RoundManager.Instance.ClearPlayerCurrentPoints();
        }
        else if (playerInfo.isWinning == true && gameState.currentLevelIndex == RoundManager.Instance.totalRounds * 3)
        {
            yield break;
        }
        // move on to next boss
        else
        {
            SceneManager.LoadScene("Level " + (gameState.currentLevelIndex + 1).ToString(), LoadSceneMode.Additive);
            gameState.currentLevelIndex++;
        }

        if (gameState.currentLevelIndex < 6)
        {
            gameState.isElvisLevel = true;
            gameState.isCorpoLevel = false;
            gameState.isCaesarLevel = false;

            // e1Cannon.SetActive(true);
        }
        else if (gameState.currentLevelIndex < 11)
        {
            gameState.isElvisLevel = false;
            gameState.isCorpoLevel = true;
            gameState.isCaesarLevel = false;

            e1Cannon.SetActive(false);
            kaibaCannon.SetActive(true);

            Cannon.UpdateEnemy();
        }
        else
        {
            gameState.isElvisLevel = false;
            gameState.isCorpoLevel = false;
            gameState.isCaesarLevel = true;

            kaibaCannon.SetActive(false);
            caesarCannon.SetActive(true);

            Cannon.UpdateEnemy();
        }

        musicController.NextLevel();

        ScoreboardUI.Instance.UpdateEnemyTitleText();
    }
}
