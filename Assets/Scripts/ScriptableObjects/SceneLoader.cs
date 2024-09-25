using ScriptableObjectArchitecture;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject {
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayerInfo playerInfo;
    public SceneReference mainMenuScene;
    public SceneReference gameScene;
    public int firstLevelIndex = 1;
    
    public void DebugTest(string testString) {
        Debug.Log("test " + testString + " at " + Time.time);
    }

    // This will load the game scene and the first level (no saving if you go back to main menu yet)
    public void LoadGameScene() {
        ResetTime();
        SceneManager.LoadScene(gameScene.ScenePath);
/*        LoadTutorial();
        gameState.currentLevelIndex = 0;*/
    }

    public void SetLevelIndex(int levelIndex) {
        gameState.currentLevelIndex = levelIndex;
    }

    public void SetIsPracticing(bool isPracticing)
    {
        playerInfo.isPracticing = isPracticing;
        playerInfo.skippedTutorial = false;
    }
    public void SetIsNotPracticing()
    {
        playerInfo.isPracticing = false;
        playerInfo.skippedTutorial = true;
    }

    // Loads levels additively (Game scene is always loaded)
    public void LoadNewLevel()
    {
        // only game scene is loaded so we can just add the first level
        /*        if (currentLevelIndex.Value < firstLevelIndex)
                {
                    SceneManager.LoadScene("Level " + firstLevelIndex.ToString(), LoadSceneMode.Additive);
                    currentLevelIndex.Value++;
                }*/
        // when we have caesar levels, make it * 3
        /*        if (gameState.currentLevelIndex == RoundManager.Instance.totalRounds * 3) return; // replace with win/lose screen
                else SceneLoaderManager.Instance.LoadNextLevel();*/

        SceneLoaderManager.Instance.LoadNextLevel();
    }

    public void LoadTutorial()=> SceneManager.LoadScene("Level 0", LoadSceneMode.Additive);

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Additive);
        
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level " + levelIndex.ToString(), LoadSceneMode.Additive);
    }

    public void SetCurrentRound(int round)
    {
        gameState.currentRound = round;
    }

    public void SetCurrentBoss(int boss)
    {
        switch (boss)
        {
            case 1:
                gameState.isElvisLevel = true;
                gameState.isCorpoLevel = false;
                gameState.isCaesarLevel = false;
                break;
            case 2:
                gameState.isElvisLevel = false;
                gameState.isCorpoLevel = true;
                gameState.isCaesarLevel = false;
                break;
            case 3:
                gameState.isElvisLevel = false;
                gameState.isCorpoLevel = false;
                gameState.isCaesarLevel = true;
                break;
        }
    }
    public void Restart()
    {
        ResetTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        ResetTime();
        SceneManager.LoadScene(mainMenuScene.ScenePath);
        gameState.currentLevelIndex = 0;
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetTime() {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}