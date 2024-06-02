using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeterData", menuName = "MeterData")]
public class MeterData : ScriptableObject
{
    public float maxValue;
    public float minValue;
    [HideInInspector] public float meterValue;
}