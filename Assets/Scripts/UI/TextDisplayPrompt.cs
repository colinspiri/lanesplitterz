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

    void Start()
    {
        RoundManager.OnNewRound += () => ChangeText(firstPrompt);
        RoundManager.OnNewThrow += () => ChangeText(firstPrompt);

        RoundManager.OnNewRound += () => EnableBackground();
        RoundManager.OnNewThrow += () => EnableBackground();

        ChangeText(firstPrompt);
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
}
