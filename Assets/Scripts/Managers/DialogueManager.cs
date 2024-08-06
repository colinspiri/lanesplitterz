using System.Collections;
using System.Collections.Generic;
using GameAudioScriptingEssentials;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    private DialogueRunner _dialogueRunner;
    [SerializeField] private AdaptiveMusicContainer gameMusic;
    [Space]
    [SerializeField] private string elvisPlayerWinDialogue;
    [SerializeField] private string elvisPlayerLossDialogue;
    [Space]
    [SerializeField] private string corpoPlayerWinDialogue;
    [SerializeField] private string corpoPlayerLossDialogue;
    [Space]
    [SerializeField] private string caesarPlayerWinDialogue;
    [SerializeField] private string caesarPlayerLossDialogue;

    private void Awake()
    {
        _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    public void PlayDialogue(string dialogueTitle) { if (_dialogueRunner && dialogueTitle != "") _dialogueRunner.StartDialogue(dialogueTitle); }

    public void PlayElvisEndDialgue()
    {
        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore)
        {
            _dialogueRunner.StartDialogue(elvisPlayerWinDialogue);
            gameMusic.TransitionSection(0);
        }

        else
        {
            _dialogueRunner.StartDialogue(elvisPlayerLossDialogue);
            gameMusic.TransitionSection(1);
        }
    }

    public void PlayCorpoEndDialgue()
    {
        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore)
        {
            _dialogueRunner.StartDialogue(corpoPlayerWinDialogue);
            gameMusic.TransitionSection(0);
        }

        else
        {
            _dialogueRunner.StartDialogue(corpoPlayerLossDialogue);
            gameMusic.TransitionSection(1);
        }
    }

    public void PlayCaesarEndDialgue()
    {
        if (RoundManager.Instance.playerFinalScore > RoundManager.Instance.enemyFinalScore)
        {
            _dialogueRunner.StartDialogue(caesarPlayerWinDialogue);
            gameMusic.TransitionSection(0);
        }

        else
        {
            _dialogueRunner.StartDialogue(caesarPlayerLossDialogue);
            gameMusic.TransitionSection(1);
        }
    }
}
