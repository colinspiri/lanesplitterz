using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using ScriptableObjectArchitecture;
using UnityEngine;

public class TextDisplayPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image background;
    [SerializeField] private TextAreaVariable firstPrompt;
    [SerializeField] private bool showOnEveryThrow;
    [SerializeField] private bool showAtBeginning;

    private void Start()
    {
        if (showOnEveryThrow)
        {
            RoundManager.OnNewRound += () => ChangeText(firstPrompt);
            RoundManager.OnNewThrow += () => ChangeText(firstPrompt);

            RoundManager.OnNewRound += EnableBackground;
            RoundManager.OnNewThrow += EnableBackground;

        }

        if (showAtBeginning)
        {
            ChangeText(firstPrompt);
            EnableBackground();            
        }
        else
        {
            ClearText();
            DisableBackground();
        }
        
        //ShowTextWithBackground(firstPrompt);
    }
    
    public void ShowTextWithBackground(TextAreaVariable newText)
    {
        promptText.text = newText.text;
        EnableBackground();
    }

    public void ChangeText(TextAreaVariable newText)
    {
        promptText.text = newText.text;
    }

    public void ClearText()
    {
        promptText.text = string.Empty;
    }

    public void EnableBackground()
    {
        background.gameObject.SetActive(true);
    }

    public void DisableBackground()
    {
        background.gameObject.SetActive(false);
    }

    public void Clear()
    {
        ClearText();
        DisableBackground();
    }
}
