using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool skippedCutscene = false;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += LoadMainMenu;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (Input.anyKeyDown && !skippedCutscene)
        {
            skippedCutscene = true;
            SkipCutscene();
        }
    }

    private void SkipCutscene()
    {
        videoPlayer.Stop();
        LoadMainMenu(videoPlayer);
    }

    private void LoadMainMenu(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}