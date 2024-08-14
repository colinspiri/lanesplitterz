using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BreakableObjects : MonoBehaviour
{
    [SerializeField]
    private GameObject BrokenCube;

    private void OnCollisionEnter(Collision collision)
    {
        BrokenCube.SetActive(true);
        gameObject.SetActive(false);
    }
}
