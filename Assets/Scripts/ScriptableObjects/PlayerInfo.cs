using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    [HideInInspector]
    public int playerCurrentPoints;
    [HideInInspector]
    public float currentFuel;
    [HideInInspector]
    public bool isPracticing;
}