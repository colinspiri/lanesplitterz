using ScriptableObjectArchitecture;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject {
    [SerializeField] private GameState gameState;
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
        LoadTutorial();
        gameState.currentLevelIndex = 0;
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
        if (gameState.currentLevelIndex == 5) return; // replace with win/lose screen
        else SceneLoaderManager.Instance.LoadNextLevel();
    }

    public void LoadTutorial()=> SceneManager.LoadScene("Level 0", LoadSceneMode.Additive);

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