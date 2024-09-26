using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject menuManager;

    void Start()
    {
        menuManager.SetActive(false); 
    }

    public void DisplayMainMenuButton()
    {
        menuManager.SetActive(true);
    }
}
