using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
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
        if (playerInfo.isPracticing == true) LoadTutorialLevel();
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
        SceneManager.LoadScene("Level " + (gameState.currentLevelIndex + 1).ToString(), LoadSceneMode.Additive);
        gameState.currentLevelIndex++;
        musicController.NextLevel(gameState.currentLevelIndex);
    }
}
