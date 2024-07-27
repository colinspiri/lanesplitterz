using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaneComponents", menuName = "LaneComponents")]
public class LaneComponents : ScriptableObject
{
    [HideInInspector]
    public GameObject ground;
    [HideInInspector]
    public GameObject waitingArea;
}
