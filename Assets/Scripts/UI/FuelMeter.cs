using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class FuelMeter : Meter
{
    public override void Start()
    {
        base.Start();
        slider.value = 1;
    }
    public void UpdateFuelMeter(FloatVariable fuel)
    {
        slider.value = fuel.Value;
    }
}
