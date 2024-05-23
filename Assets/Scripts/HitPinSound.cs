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
    [SerializeField] private float wait = 0.15f;
    
    private void Start() {
        PinManager.OnPinKnockedDown += PlayPinHitSound;
    }

    private void PlayPinHitSound() {
        pinHit.Play();
        StartCoroutine(PinsFall());

        // remove callback so it only happens the first time you hit a pin
        PinManager.OnPinKnockedDown -= PlayPinHitSound;
    }

    private IEnumerator PinsFall()
    {
        yield return new WaitForSeconds(wait);
        if (pinsKnockedDown.Value > 5)
        {
            bigFall.Play();
        }
        else
        {
            smallFall.Play();
        }
    }
}
