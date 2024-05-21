using System.Collections.Generic;
using UnityEngine;

public abstract class ActionOnCollide : MonoBehaviour {
    [SerializeField] private List<string> possibleTags;
    [SerializeField] private int triggerCount = -1;
    
    private int _triggerCounter;

    private void Awake() {
        _triggerCounter = triggerCount;
    }

    protected abstract void DoAction();

    private void OnCollisionEnter(Collision collision) {
        if (_triggerCounter is > 0 or -1) {
            bool hitObjectWithTag = false;
            foreach (var tag in possibleTags) {
                if (collision.gameObject.CompareTag(tag)) {
                    hitObjectWithTag = true;
                    break;
                }
            }
            
            if (hitObjectWithTag) {
                DoAction();

                if (_triggerCounter > 0) {
                    _triggerCounter--;
                }
            }
        }
    }
}