using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject {
    public SceneReference mainMenuScene;
    public SceneReference gameScene;
    
    public void DebugTest(string testString) {
        Debug.Log("test " + testString + " at " + Time.time);
    }

    public void LoadGameScene() {
        ResetTime();
        SceneManager.LoadScene(gameScene.ScenePath);
    }

    public void Restart()
    {
        ResetTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        ResetTime();
        SceneManager.LoadScene(mainMenuScene.ScenePath);
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