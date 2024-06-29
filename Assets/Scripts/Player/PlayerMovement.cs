using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Object Variables

    public static PlayerMovement Instance;
    private Rigidbody _myBody;
    private Transform _myCam;
    private CameraFollowObject _myCamHandle;
    private Quaternion _camInvRot;
    private SphereCollider _myCollider;
    private float _myRadius;
    
    #endregion

    #region Action Flags

    [Header("Configuration Flags")]
    public bool acceptingInputs = true;
    public bool disableOnStart = true;
    public bool enableFuelLoss = true;
    private float _turnVal;
    private float _accelVal;
    
    #endregion
    
    [Header("Force specifications")]
    [SerializeField] private float accelForce;
    [SerializeField] private float turnForce;
    [SerializeField] private float slipperyForce = 10f;
    [SerializeField] private float brakeDelay;
    [SerializeField] private float minimumSpeed;

    [Header("Spin")] 
    [Tooltip("Every frame, hookForce is multiplied by the spin [-100, 100] to turn the ball based on the spin")]
    [SerializeField] private float hookForceMultiplier;
    [Tooltip("While turning, spin value [-100, 100] is modified each second by this value")] 
    [SerializeField] private float turnSpeedPerSecond = 0.1f;

    [Header("Fuel specifications")]
    [Tooltip("Amount of fuel expended per second while accelerating (not decelerating)")]
    [SerializeField] private float accelFuel;
    [Tooltip("Amount of fuel expended per second while steering left or right")]
    [SerializeField] private float turnFuel;
    
    [Header("Events")]
    [SerializeField] private FloatVariableGameEvent fuelChanged;

    [Header("Rigidbody configuration")]
    [SerializeField] private float maxLinearVelocity = 1e+16f;
    [SerializeField] private float maxAngularVelocity = 50f;

    [Header("Audio Sources")]
    [SerializeField] AudioSource BallRolling;

    // public properties
    [HideInInspector] public int TurnDirection; // -1 is left, 1 is right, 0 is not turning
    [HideInInspector] public int AccelerationDirection; // -1 is decelerating, 1 is accelerating, 0 is no acceleration

    // private state
    [SerializeField] private FloatVariable currentFuel;
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;
    private float _fuelMeter = 1f;
    private float _currentSpeed = 0f;
    private GameObject _ground = null;
    private bool _hasLaunched;
    private float _currentSpin = 0f;

    // misc
    private int _groundMask;

    #region MonoBehaviour Functions

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        
        _myCam = GameObject.FindWithTag("MainCamera").transform;
        _myCamHandle = _myCam.GetComponentInParent<CameraFollowObject>();
        
        _myCollider = GetComponent<SphereCollider>();
        _myCollider.hasModifiableContacts = true;

        _myRadius = _myCollider.radius * transform.lossyScale.x;

        _groundMask = LayerMask.GetMask("Ground");
        
        // Setting max parameters of rigidbody
        _myBody.maxLinearVelocity = maxLinearVelocity;
        _myBody.maxAngularVelocity = maxAngularVelocity;

        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
        currentFuel.Value = 1;

        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
        RoundManager.OnNewThrow += () => _hasLaunched = false;
        RoundManager.OnNewRound += () => _hasLaunched = false;

        Initialize();
    }

    private void Initialize() {
        if (disableOnStart) gameObject.SetActive(false);
        
        transform.position = _startingPosition;
        transform.rotation = _startingRotation;
        
        _myBody.velocity = Vector3.zero;
        _myBody.angularVelocity = Vector3.zero;

        _myCamHandle.DefaultForward();

        currentFuel.Value = 1;
        _fuelMeter = 1f;
    }

    private void Update()
    {
        // Update camera inverse transform
        Quaternion camRotation = _myCam.rotation;
        _camInvRot = Quaternion.Inverse(new Quaternion(camRotation.x, 0f, camRotation.z, camRotation.w));
        
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Alpha0)) {
            Initialize();
        }

        if (acceptingInputs)
        {
            _turnVal = Input.GetAxis("Turn");

            _accelVal = Input.GetAxis("Accelerate");
            
            // Input value between 0 and -1 if decelerating
            if (_accelVal > 0f) _accelVal *= accelForce;
        }
        else
        {
            _turnVal = 0f;
            _accelVal = 0f;
        }
        
        // set public-accessible acceleration state
        if (_accelVal < 0) AccelerationDirection = -1;
        else if (_accelVal > 0) AccelerationDirection = 1;
        else AccelerationDirection = 0;
                
        // set public-accessible turn state
        if (_turnVal < 0) TurnDirection = -1;
        else if (_turnVal > 0) TurnDirection = 1;
        else TurnDirection = 0;
    }
    
    private void FixedUpdate()
    {
        UpdateGround();

        // Debug.Log("currentSpin = " + _currentSpin);
        Hook();
        
        if (Mathf.Abs(_turnVal) > Mathf.Epsilon) Turn(_turnVal);
        
        if (Mathf.Abs(_accelVal) > Mathf.Epsilon) Accelerate(_accelVal);

        // Keep ball from going too slow
        _currentSpeed = Vector3.Project(_myBody.velocity, _myCam.forward).magnitude;
        if (_hasLaunched && _currentSpeed < minimumSpeed) Accelerate(1f, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Negate force of collision against pin
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Pins"))
        {
            ContactPoint[] contactList = new ContactPoint[collision.contactCount];
        
            collision.GetContacts(contactList);

            foreach (ContactPoint contact in contactList)
            {
                _myBody.AddForceAtPosition(-contact.impulse, contact.point, ForceMode.Impulse);
            }
        }
    }

    #endregion

    #region Movement Functions

    // Emulate frictional movement to the side
    private void Hook()
    { 
        if (!Grounded() || IsIcy()) return;

        float hookForceMagnitude = _currentSpin * hookForceMultiplier;
        Vector3 hookForce = (_camInvRot * _myCam.right) * hookForceMagnitude;
        
        _myBody.AddForce(hookForce);

        /*Vector3 camUp = _camInvRot * _myCam.up; 
        float force = Vector3.Dot(_myBody.angularVelocity, camUp.normalized); 
        Turn(force * hookMultiplier, false);*/
    }
    
    // turnVal is turn force, negative for left, positive for right
    public void Turn(float turnVal, bool expendFuel = true)
    {
        if (expendFuel)
        {
            float fuelReduction = turnFuel * Time.fixedDeltaTime;

            if (_fuelMeter <= Mathf.Epsilon)
            {
                return;
            }
            else
            { 
                ReduceFuel(fuelReduction);
            }
        }

        _currentSpin += turnVal * turnSpeedPerSecond * Time.deltaTime;
        if (_currentSpin > 100) _currentSpin = 100;
        else if (_currentSpin < -100) _currentSpin = -100;

        /*
        turnVal *= turnForce;
        
        Vector3 linForce = (_camInvRot * _myCam.right) * turnVal;
        Vector3 rotForce = (_camInvRot * _myCam.up) * turnVal;
        
        // Skid if on ice (reduce acceleration)
        if (IsIcy())
        {
            Debug.Log("Turning skidding on ice");
            
            linForce /= slipperyForce;
            rotForce /= slipperyForce;
        }
        
        // Linear acceleration
        _myBody.AddForce(linForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotForce, ForceMode.Impulse);*/
    }
    
    // accelVal is the force to accelerate with
    public void Accelerate(float accelVal, bool expendFuel = true)
    {
        Vector3 linearForce;
        Vector3 rotationalForce;

        Vector3 camForward = (_camInvRot * _myCam.forward).normalized;
        Vector3 camRight = (_camInvRot * _myCam.right).normalized;
        
        // Put on the brakes
        if (accelVal < 0)
        {
            Vector3 forwardLinVelocity = Vector3.Project(_myBody.velocity, camForward);
            linearForce = forwardLinVelocity * accelVal / brakeDelay;
            
            Vector3 forwardRotVelocity = Vector3.Project(_myBody.angularVelocity, camRight);
            rotationalForce = forwardRotVelocity * accelVal / brakeDelay;
        }
        // Accelerate ahead
        else
        {
            if (expendFuel)
            {
                float fuelReduction = accelFuel * Time.fixedDeltaTime;

                if (_fuelMeter <= Mathf.Epsilon)
                {
                    return;
                }
                else
                {
                    ReduceFuel(fuelReduction);
                }
            }
        
            linearForce = camForward * accelVal;
            rotationalForce = camRight * accelVal;
        }

        // Skid if on ice (reduce acceleration)
        if (IsIcy())
        {
            // Debug.Log("Acceleration skidding on ice");
            
            linearForce /= slipperyForce;
            rotationalForce /= slipperyForce;
        }
        
        // Linear acceleration
        _myBody.AddForce(linearForce, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque(rotationalForce, ForceMode.Impulse);
    }
    
    // Overload for acceleration in an arbitrary direction
    // Forces velocity along new direction, then adds extra force
    /* Can probably be split into multiple methods (See Steer() below) */
    public void Accelerate(Vector3 accelForce, bool expendFuel = true)
    {
        if (expendFuel)
        {
            float fuelReduction = accelFuel * Time.fixedDeltaTime;

            if (_fuelMeter <= Mathf.Epsilon)
            {
                return;
            }
            else
            {
                ReduceFuel(fuelReduction);
            }
        }
        
        Vector3 camUp = (_camInvRot * _myCam.up).normalized;
        Vector3 linearForce = accelForce;
        Vector3 rotationalForce = Vector3.Cross(accelForce, camUp);

        // Skid if on ice (reduce acceleration)
        if (IsIcy())
        {
            // Debug.Log("Acceleration skidding on ice");
            
            linearForce /= slipperyForce;
            rotationalForce /= slipperyForce;
        }
        
        // Steer existing velocity toward direction of force
        _myBody.velocity = linearForce.normalized * _myBody.velocity.magnitude;
        _myBody.angularVelocity = rotationalForce.normalized * _myBody.angularVelocity.magnitude;
        
        // Linear acceleration 
        _myBody.AddForce(linearForce, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque(rotationalForce, ForceMode.Impulse);
    }

    // Force velocity along a new direction
    /* TO DO */
    /* Make this gradual later (through coroutines) */
    /* Increase acceleration overload efficiency with this */
    public void Steer(Vector3 linDir)
    {
        Vector3 camUp = (_camInvRot * _myCam.up).normalized;
        Vector3 rotDir = Vector3.Cross(linDir, camUp);
        
        _myBody.velocity = linDir.normalized * _myBody.velocity.magnitude;
        _myBody.angularVelocity = rotDir.normalized * _myBody.angularVelocity.magnitude;
    }

    // Adds spin to the ball
    // Positive spinVal spins CW (left), negative spins CCW (right)
    // Magnitude of spinVal determines magnitude of torque
    public void Spin(float spinVal) {
        _currentSpin = spinVal * -1;
        //_myBody.AddTorque((_camInvRot * _myCam.forward) * spinVal, ForceMode.Impulse);
    }

    
    #endregion

    #region Support Functions

    // Updates the internal ground variable for keeping track of current terrain
    /* Will need to be updated if we ever add curved terrain with different normals */
    private void UpdateGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (_camInvRot * _myCam.up) * -1f, out hit,
                _myCollider.radius + 1f, _groundMask))
        {
            if (!Grounded())
            {
                BallRolling.Play();
            }
            _ground = hit.collider.gameObject;
        }
        else
        {
            _ground = null;
            BallRolling.Stop();
        }
    }
    
    // Returns true if on the ground, false otherwise
    private bool Grounded()
    {
        return _ground;
    }

    //Returns true is on icy ground, false if not
    private bool IsIcy()
    {
        return _ground && _ground.CompareTag("Icy");
    }

    // Deplete the fuel meter by some percent (between 0 and 1)
    public void ReduceFuel(float fuelPercent)
    {
        if (enableFuelLoss)
        {
            _fuelMeter -= Mathf.Clamp(fuelPercent, 0f, 1f);

            _fuelMeter = Mathf.Clamp(_fuelMeter, 0f, 1f);

            currentFuel.Value -= fuelPercent;

            currentFuel.Value = Mathf.Clamp(currentFuel.Value, 0f, 1f);

            fuelChanged.Raise(currentFuel);
        }
    }

    // Increase the fuel meter by some percent (between 0 and 1)
    public void RestoreFuel(float fuelPercent)
    {
        _fuelMeter += Mathf.Clamp(fuelPercent, 0f, 1f);
        
        _fuelMeter = Mathf.Clamp(_fuelMeter, 0f, 1f);

        currentFuel.Value += fuelPercent;

        fuelChanged.Raise(currentFuel);
    }

    public void SetBallLaunched() { StartCoroutine(BallLaunchedCoroutine()); }

    private IEnumerator BallLaunchedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _hasLaunched = true;
    }

    // Negate collision force against pins
    // Assuming fixedupdate will never be interrupted by a collision call
    // private void NegateCollisions()
    // {
    //     if (_hasCollided)
    //     {
    //         foreach (ContactPoint[] contactList in _collisionContacts)
    //         {
    //             foreach (ContactPoint contact in contactList)
    //             {
    //                 _myBody.AddForceAtPosition(-contact.impulse, contact.point, ForceMode.Impulse);
    //             }
    //         }
    //         
    //         _collisionContacts.Clear();
    //         _hasCollided = false;
    //     }
    // }
    
    #endregion
}
