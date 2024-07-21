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
    [SerializeField] private IntVariable currentLevelIndex;
    
    private void Awake()
    {
        Instance = this;
    }
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelCoroutine());
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
        yield return SceneManager.UnloadSceneAsync("Level " + currentLevelIndex.Value.ToString());
        SceneManager.LoadScene("Level " + (currentLevelIndex.Value + 1).ToString(), LoadSceneMode.Additive);
        currentLevelIndex.Value++;
    }
}
