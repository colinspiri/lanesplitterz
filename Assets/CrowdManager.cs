using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{

    [SerializeField] private AudioClipRandomizer crowdRoar;
    public float crowdWait = 10.0f;
    
    void Start()
    {
        PinManager.OnPinHitByBall += CrowdRoar;
    }

    private void CrowdRoar()
    {
        PinManager.OnPinHitByBall -= CrowdRoar;
        crowdRoar.PlaySFX();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(crowdWait);
        PinManager.OnPinHitByBall += CrowdRoar;
    }
}
