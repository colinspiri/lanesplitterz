using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour {
    public static PauseMenuManager Instance;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private List<GameObject> objectsToDisableOnPause;
    private List<bool> _wereObjectsActiveBeforePause = new();
    
    private PlayerInputActions _inputActions;

    private void Awake() {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        
        foreach (var t in objectsToDisableOnPause) {
            _wereObjectsActiveBeforePause.Add(t.activeSelf);
        }
        ClosePauseMenu();
    }
    
    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;
        
        if (_inputActions.UI.Cancel.triggered)
        {
            if (pauseMenu.activeSelf) ClosePauseMenu();
            else OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        
        for (int i = 0; i < objectsToDisableOnPause.Count; i++) {
            _wereObjectsActiveBeforePause[i] = objectsToDisableOnPause[i].activeSelf;
            objectsToDisableOnPause[i].SetActive(false);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.Pause();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        
        for (int i = 0; i < objectsToDisableOnPause.Count; i++) {
            objectsToDisableOnPause[i].SetActive(_wereObjectsActiveBeforePause[i]);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.Resume();
    }
}