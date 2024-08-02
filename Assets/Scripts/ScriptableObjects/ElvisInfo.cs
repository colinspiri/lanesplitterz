using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ElvisInfo", menuName = "ElvisInfo")]
public class ElvisInfo : SerializedScriptableObject
{
    [Tooltip("Key should be the round, value should be the throw")]
    public Dictionary<int, int> doublePointsThrows;
}