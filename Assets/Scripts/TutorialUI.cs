using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using SOCodeGeneration.CODE_GENERATION.Variables;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialSequenceVariable sequence;
    [SerializeField] private TextDisplayPrompt topLeftPrompt;
    [SerializeField] private TextDisplayPrompt topRightPrompt;
    [SerializeField] private TextDisplayPrompt centerPrompt;
    [SerializeField] private TextDisplayPrompt rightPrompt;

    [Header("Prompts")]
    [SerializeField] private TextAreaVariable steerPrompt;
    [SerializeField] private TextAreaVariable steerFuelPrompt;
    [SerializeField] private TextAreaVariable acceleratePrompt;
    [SerializeField] private TextAreaVariable obstaclePrompt;
    [SerializeField] private TextAreaVariable goldenPinPrompt;

    [Header("Game Events")]
    [SerializeField] private GameEvent startFuelTutorialDialogue;
    [SerializeField] private GameEvent startObstacleTutorialDialogue;
    [SerializeField] private GameEvent startPinTutorialDialogue;

    private void Start()
    {
        sequence.AddListener(OnSequenceChanged);
    }

    private void OnDestroy()
    {
        sequence.RemoveListener(OnSequenceChanged);
    }

    private void OnSequenceChanged(TutorialSequence seq)
    {
        switch (seq)
        {
            case TutorialSequence.None or TutorialSequence.ReadyForNext:
                topLeftPrompt.Clear();
                topRightPrompt.Clear();
                centerPrompt.Clear();
                rightPrompt.Clear();
                break;
            case TutorialSequence.Steer:
                startFuelTutorialDialogue.Raise();
                break;
            case TutorialSequence.Accelerate:
                topLeftPrompt.ShowTextWithBackground(acceleratePrompt);
                topRightPrompt.Clear();
                centerPrompt.Clear();
                rightPrompt.Clear();
                break;
            case TutorialSequence.Obstacle:
                startObstacleTutorialDialogue.Raise();
                break;
            case TutorialSequence.GoldenPin:
                startPinTutorialDialogue.Raise();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(seq), seq, null);
        }
    }
}
