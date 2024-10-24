using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{

    [SerializeField] private AudioClipRandomizer crowdRoar;
    [SerializeField] private AudioSource crowdBoo;
    public float crowdWait = 10.0f;
    
    void Start()
    {
        PinManager.OnPinHitByBall += CrowdRoar;
    }

    private void CrowdRoar()
    {
        if (PinManager.Instance.player)
        {
            PinManager.OnPinHitByBall -= CrowdRoar;
            crowdRoar.PlaySFX();
            StartCoroutine(Wait());
        }
    }

    public void CrowdBoo()
    {
        crowdBoo.Play();
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(crowdWait);
        PinManager.OnPinHitByBall += CrowdRoar;
    }
}
