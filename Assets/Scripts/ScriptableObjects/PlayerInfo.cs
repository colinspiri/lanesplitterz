using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    // not including playerCurrentPoints because it uses TextDisplayInt.cs which requires it to be an IntVariable scriptable object
    // it felt kinda useless to nest the scriptable objects in this case

    [HideInInspector]
    public float currentFuel;
    [HideInInspector]
    public bool isPracticing;
    [HideInInspector] 
    public bool skippedTutorial;
    [HideInInspector]
    public bool isWinning;
    [HideInInspector]
    public bool isReady;
    [HideInInspector]
    public bool pressedContinue;
    [HideInInspector]
    public bool finishedTutorial;
}