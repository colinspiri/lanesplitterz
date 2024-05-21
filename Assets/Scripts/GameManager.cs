using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [SerializeField] private IntVariable currentPoints;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        currentPoints.Value = 0;
    }
}