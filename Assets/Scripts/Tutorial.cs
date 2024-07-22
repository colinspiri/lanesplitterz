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
    
    private bool _started;
    private bool _playerHitObstacle;
    
    public void BeginTutorial()
    {
        if (_started) return;
        _started = true;
        StartCoroutine(DoTutorial());
    }
    
    private IEnumerator DoTutorial()
    {
        yield return new WaitForSeconds(waitBeforeSteerPrompt);
        sequence.SetValue(TutorialSequence.Steer);
        TimeManager.Instance.PauseForTutorial();
        yield return new WaitForInput(new []{KeyCode.A, KeyCode.D});
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);
        
        yield return new WaitForSeconds(waitBeforeAcceleratePrompt);
        sequence.SetValue(TutorialSequence.Accelerate);
        TimeManager.Instance.PauseForTutorial();
        yield return new WaitForInput(KeyCode.W);
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);
        
        yield return new WaitUntil(() => PlayerMovement.Instance.transform.position.z > zThresholdForGoldenPinsPrompt);
        sequence.SetValue(TutorialSequence.GoldenPin);
        TimeManager.Instance.PauseForTutorial();
        // slight flaw - prompt counter will countdown during pause menu
        yield return new WaitForSecondsRealtime(waitWhenAtGoldenPins);
        TimeManager.Instance.ResumeFromTutorialPause();
        sequence.SetValue(TutorialSequence.ReadyForNext);
    }

    public void OnPlayerHitObstacle()
    {
        if (_playerHitObstacle) return;
        _playerHitObstacle = true;
        StartCoroutine(NotifyObstacleEffects());
    }

    private IEnumerator NotifyObstacleEffects()
    {
        sequence.SetValue(TutorialSequence.Obstacle);
        TimeManager.Instance.PauseForTutorial();
        // slight flaw - prompt counter will countdown during pause menu
        yield return new WaitForSecondsRealtime(waitWhenObstacleHit);
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
