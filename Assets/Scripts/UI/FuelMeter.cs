using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class FuelMeter : Meter {
    [SerializeField] private float remainTime = 1;
    private float _remainTimer;
    
    public override void Start()
    {
        base.Start();
        slider.value = 1;
    }
    public void UpdateFuelMeter(FloatVariable fuel)
    {
        slider.value = fuel.Value;
        
        EnableMeter();
        _remainTimer = remainTime;
    }

    private void Update() {
        if (_remainTimer > 0) {
            _remainTimer -= Time.deltaTime;
        }
        else {
            DisableMeter();
        }
    }
}
