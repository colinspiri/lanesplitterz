using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour {
    public static PauseMenuManager Instance;

    [SerializeField] private GameObject pauseMenu;
    private PlayerInputActions _inputActions;
    
    private bool _paused;
    
    private void Start()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        
        ClosePauseMenu();
    }
    
    private void Update()
    {
        if (_paused) return;
        
        if (_inputActions.UI.Cancel.triggered)
        {
            if (pauseMenu.activeSelf) ClosePauseMenu();
            else OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Pause();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        

        Resume();
    }
    
    private void Pause() {
        Time.timeScale = 0;
    }

    private void Resume() {
        Time.timeScale = 1;
    }
}