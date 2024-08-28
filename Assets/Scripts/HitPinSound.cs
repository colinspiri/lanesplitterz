using System;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAudioScriptingEssentials;

public class HitPinSound : MonoBehaviour
{
    // components
    [SerializeField] private IntReference pinsKnockedDown;
    [SerializeField] private AudioClipRandomizer pinHitSlow;
    [SerializeField] private AudioClipRandomizer pinHitNormal;
    [SerializeField] private AudioClipRandomizer bigFall;
    [SerializeField] private AudioClipRandomizer smallFall;
    [SerializeField] bool isPlayer;
    
    // parameters

    // controls how long it waits to count number of fallen pins and then plays sound
    [SerializeField] private float countWait = 0.15f;
    // controls how long it waits until playing the whole sound again (hitting another group of pins)
    [SerializeField] private float nextHitWait = 0.5f;
    [SerializeField] private int pinNumber = 5;

    private int currentlyKnockedDown = 0;
    private bool slow = true;
    
    private void Start() {
        PinManager.OnPinHitByBall += PlayPinHitSound;
        RoundManager.OnNewThrow += ResetThrow;
        RoundManager.OnNewRound += ResetThrow;
    }

    private void PlayPinHitSound() {
        Debug.Log("Was player hit: " + PinManager.Instance.player);
        if (PinManager.Instance.player == isPlayer)
        {
            if (slow && isPlayer)
            {
                pinHitSlow.PlaySFX();
                slow = false;
            }
            else
            {
                pinHitNormal.PlaySFX();
            }
            StartCoroutine(PinsFall());

            // remove callback so it only happens the first time you hit a pin
            PinManager.OnPinHitByBall -= PlayPinHitSound;
        }
        
    }

    private void ResetThrow()
    {
        currentlyKnockedDown = 0;
        slow = true;
    }

    private IEnumerator PinsFall()
    {
        currentlyKnockedDown = pinsKnockedDown.Value;
        yield return new WaitForSeconds(countWait);
        if (pinsKnockedDown.Value - currentlyKnockedDown > pinNumber)
        {
            bigFall.PlaySFX();
        }
        else
        {
            smallFall.PlaySFX();
        }
        yield return new WaitForSeconds(nextHitWait);
        PinManager.OnPinHitByBall += PlayPinHitSound;
    }

}
