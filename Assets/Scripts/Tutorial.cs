using System.Collections;
using System.Collections.Generic;
using SOCodeGeneration.CODE_GENERATION.Variables;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TutorialSequenceVariable sequence;
    
    [SerializeField] private float waitBeforeSteerPrompt;
    [SerializeField] private float waitBeforeAcceleratePrompt;
    [SerializeField] private float waitWhenObstacleHit;
    [SerializeField] private float waitWhenAtGoldenPins;
    [SerializeField] private float zThresholdForGoldenPinsPrompt;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    private bool _started;
    private bool _playerHitObstacle;
    
    // NOTE: This gets called when the LaunchedBall Game Event is raised
    public void BeginTutorial()
    {
        if (!playerInfo.isPracticing) return;
        if (_started) return;
        _started = true;
        StartCoroutine(DoTutorial());
    }
    
    private IEnumerator DoTutorial()
    {
        yield return new WaitForSeconds(waitBeforeSteerPrompt);
        sequence.SetValue(TutorialSequence.Steer);
        TimeManager.Instance.PauseForTutorial();
        yield return new WaitUntil(() => gameState.isDialogueRunning == false);
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);

        yield return new WaitUntil(() => PlayerMovement.Instance.transform.position.z > zThresholdForGoldenPinsPrompt);
        sequence.SetValue(TutorialSequence.GoldenPin);
        TimeManager.Instance.PauseForTutorial();
        // slight flaw - prompt counter will countdown during pause menu
        yield return new WaitUntil(() => gameState.isDialogueRunning == false);
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);
    }

    public void OnPlayerHitObstacle()
    {
        if (!playerInfo.isPracticing) return;
        if (_playerHitObstacle) return;
        _playerHitObstacle = true;
        StartCoroutine(NotifyObstacleEffects());
    }

    private IEnumerator NotifyObstacleEffects()
    {
        sequence.SetValue(TutorialSequence.Obstacle);
        TimeManager.Instance.PauseForTutorial();
        // slight flaw - prompt counter will countdown during pause menu
        yield return new WaitUntil(() => gameState.isDialogueRunning == false);
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);
    }
}

public enum TutorialSequence
{
    None,
    ReadyForNext,
    Steer,
    Accelerate,
    Obstacle,
    GoldenPin
}
