using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;

public class SetEnabledFromBoolVariable : MonoBehaviour {
    public BoolVariable boolReference;
    [SerializeField] private GameObject gameObjectToSetEnabled;
    
    // state
    private bool _previousValue = false;

    private void Start() {
        UpdateEnabled();
    }

    private void Update() {
        if (boolReference.Value != _previousValue) {
            UpdateEnabled();
        }
    }

    private void UpdateEnabled() {
        gameObjectToSetEnabled.SetActive(boolReference.Value);
        _previousValue = boolReference.Value;
    }
}