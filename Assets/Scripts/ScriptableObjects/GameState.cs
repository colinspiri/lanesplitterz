using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject
{
    [HideInInspector] public int currentThrow;
    [HideInInspector] public int currentRound;
    [HideInInspector] public int currentLevelIndex;
    [HideInInspector] public bool isScoreboardEnabled;
    [HideInInspector] public bool isClearingPins;
    [HideInInspector] public bool isDoublePointsThrow;
}