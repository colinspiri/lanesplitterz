using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using BehaviorDesigner.Runtime.Tasks;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Rendering;
using Action = System.Action;

public class PinManager : MonoBehaviour {
    public static PinManager Instance;
    
    // components 
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private GameObject goldenPinPrefab;

    // scriptable object variables
    [SerializeField] private PinCollection allPins;
    [SerializeField] private PinCollection pinsStanding;
    [SerializeField] private PinCollection pinsFallen;
    [SerializeField] private IntVariable pinsKnockedDown;
    [SerializeField] private IntVariable currentPoints;
    
    // state
    private List<PinSpawn> _pinSpawns = new List<PinSpawn>();

    // public static Action OnPinKnockedDown;
    public static Action OnPinHitByBall;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pinsKnockedDown.Value = 0;
    }

    private void Update() {
        
        // debug keybindings to test functionality
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            ClearFallenPins();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            ResetAllPins();
        }
    }

    // called by Pins to tell PinManager that it's been knocked down. PinManager handles the counting and points
    public void NotifyPinKnockedDown(Pin pin) {
        pinsStanding.Remove(pin);
        pinsFallen.Add(pin);
        
        pinsKnockedDown.Value++;
        currentPoints.Value += pin.PointValue;
    }

    public void NotifyPinHitByBall(Pin pin)
    {
        OnPinHitByBall?.Invoke();
    }
    
    private void ClearFallenPins() {
        pinsFallen.DestroyAll();
        
        // reset all standing pins to starting orientations (in case it was knocked a little but didnt fall over)
        foreach (var pinSpawn in _pinSpawns) {
            var pin = pinSpawn.spawnedPinReference;
            if(pin == null) continue;

            pin.transform.position = pinSpawn.startingPosition;
            pin.transform.rotation = pinSpawn.startingRotation;
        }
    }

    private void ResetAllPins() {
        allPins.DestroyAll();
        
        // instantiate new pins at starting positions
        foreach (var pinSpawn in _pinSpawns) {
            var spawnedPin = Instantiate(pinSpawn.pinPrefab, pinSpawn.startingPosition, pinSpawn.startingRotation);
            pinSpawn.spawnedPinReference = spawnedPin;
        }
    }

    public void AddPin(Pin newPin) {
        allPins.Add(newPin);
        pinsStanding.Add(newPin);

        PinSpawn newPinSpawn = new PinSpawn {
            pinPrefab = GetPrefabFromPinType(newPin.pinType),
            startingPosition = newPin.transform.position,
            startingRotation = newPin.transform.rotation,
            spawnedPinReference = newPin.gameObject,
        };

        // add to list of PinSpawns only if no duplicates are found
        foreach (var spawn in _pinSpawns) {
            if (spawn.startingPosition == newPinSpawn.startingPosition) return;
        }
        _pinSpawns.Add(newPinSpawn);
    }

    public void DestroyPin(Pin destroyedPin) {
        allPins.Remove(destroyedPin);
        pinsStanding.Remove(destroyedPin);
        pinsFallen.Remove(destroyedPin);
    }
    
    private GameObject GetPrefabFromPinType(PinType pinType) {
        return pinType switch {
            PinType.Pin => pinPrefab,
            PinType.GoldenPin => goldenPinPrefab,
            _ => null
        };
    }

}

public enum PinType { Pin, GoldenPin }

public class PinSpawn {
    public GameObject pinPrefab;
    public Vector3 startingPosition;
    public Quaternion startingRotation;

    public GameObject spawnedPinReference;
    
    public string ToString() {
        return "PinSpawn: " + (pinPrefab == null ? "NULL" : pinPrefab.name) + " at " + startingPosition.ToString();
    }
}

