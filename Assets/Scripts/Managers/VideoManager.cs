using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += LoadMainMenu;
        videoPlayer.Play();
    }

    private void LoadMainMenu(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}