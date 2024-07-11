using System.Collections.Generic;
using UnityEngine;

public abstract class ActionOnCollide : MonoBehaviour {
    [SerializeField] private List<string> possibleTags;
    [SerializeField] private List<string> possibleLayers;
    [SerializeField] private int triggerCount = -1;
    
    private int _triggerCounter;

    protected virtual void Start() {
        Initialize();
        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
    }

    private void Initialize() {
        _triggerCounter = triggerCount;
    }

    protected virtual void DoAction() { }
    protected virtual void DoAction(Collision collision = null) { }

    private void OnCollisionEnter(Collision collision) {
        if (_triggerCounter is > 0 or -1) {
            bool hitValidObject = false;
            
            // Check object tags against list
            foreach (var tag in possibleTags) {
                if (collision.gameObject.CompareTag(tag)) {
                    hitValidObject = true;
                    break;
                }
            }

            // Check object layer against list
            if (!hitValidObject)
            {
                foreach (string layer in possibleLayers) {
                    if (collision.gameObject.layer == LayerMask.NameToLayer(layer)) {
                        hitValidObject = true;
                        break;
                    }
                }
            }
            
            // If valid tag or layer is found, perform action
            if (hitValidObject) {
                DoAction();
                DoAction(collision);

                if (_triggerCounter > 0) {
                    _triggerCounter--;
                }
            }
        }
    }
}