using System;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPinSound : MonoBehaviour
{
    // components
    [SerializeField] private IntReference pinsKnockedDown;
    [SerializeField] private AudioSource pinHit;
    [SerializeField] private AudioSource bigFall;
    [SerializeField] private AudioSource smallFall;
    
    // parameters

    // controls how long it waits to count number of fallen pins and then plays sound
    [SerializeField] private float countWait = 0.15f;
    // controls how long it waits until playing the whole sound again (hitting another group of pins)
    [SerializeField] private float nextHitWait = 0.5f;
    [SerializeField] private int pinNumber = 5;

    private int currentlyKnockedDown = 0;
    
    private void Start() {
        PinManager.OnPinHitByBall += PlayPinHitSound;
        RoundManager.OnNewThrow += ResetThrow;
    }

    private void PlayPinHitSound() {
        pinHit.Play();
        StartCoroutine(PinsFall());

        // remove callback so it only happens the first time you hit a pin
        PinManager.OnPinHitByBall -= PlayPinHitSound;
    }

    private void ResetThrow()
    {
        currentlyKnockedDown = 0;
    }

    private IEnumerator PinsFall()
    {
        currentlyKnockedDown = pinsKnockedDown.Value;
        yield return new WaitForSeconds(countWait);
        if (pinsKnockedDown.Value - currentlyKnockedDown > pinNumber)
        {
            bigFall.Play();
        }
        else
        {
            smallFall.Play();
        }
        yield return new WaitForSeconds(nextHitWait);
        PinManager.OnPinHitByBall += PlayPinHitSound;
    }

}
