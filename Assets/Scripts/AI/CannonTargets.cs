using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTargets : MonoBehaviour
{
    private static CannonTargets _instance;

    [SerializeField] private bool repeats = false;

    private void OnEnable()
    {
        if (_instance != null) Destroy(_instance.gameObject);
        
        _instance = this;
    }

    public static Vector3 GetTarget()
    {
        if (_instance.transform.childCount == 0)
        {
            Debug.LogError("CannonTargets Error: Target requested with none available");
        }

        int childIndex = Random.Range(0, _instance.transform.childCount);

        Transform child = _instance.transform.GetChild(childIndex);

        Vector3 returnVal = child.position;

        if (!_instance.repeats) Destroy(child.gameObject);

        return returnVal;
    }
}
