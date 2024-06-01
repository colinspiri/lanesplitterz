using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Object Variables

    public static PlayerMovement Instance;
    private Rigidbody _myBody;
    private Transform _myCam;
    private Quaternion _camInvRot;
    private SphereCollider _myCollider;
    
    #endregion

    #region Action Flags

    public bool acceptingInputs = true;
    public bool disableOnStart = true;
    private bool _jumpRequest;
    private bool _strafeLeft;
    private bool _strafeRight;
    private float _turnVal;
    private float _accelVal;
    
    #endregion
    
    [Header("Force specifications")]
    [SerializeField] private float accelForce;
    [SerializeField] private float turnForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float strafeForce;
    [SerializeField] private float slipperyForce = 10f;
    [SerializeField] private float brakeDelay;
    
    [Header("Rigidbody configuration")]
    [SerializeField] private float maxLinearVelocity = 1e+16f;
    [SerializeField] private float maxAngularVelocity = 50f;

    // public properties
    [HideInInspector] public int TurnDirection; // -1 is left, 1 is right, 0 is not turning
    [HideInInspector] public int AccelerationDirection; // -1 is decelerating, 1 is accelerating, 0 is no acceleration
    
    // private state
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;

    #region MonoBehaviour Functions

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        
        _myCam = GameObject.FindWithTag("MainCamera").transform;
        _camInvRot = Quaternion.Inverse(_myCam.rotation);
        /* TODO: Remove y component of inverse rotation */
        
        _myCollider = GetComponent<SphereCollider>();
        if (!_myBody || !_myCam || !_myCollider)
        {
            Debug.LogError("PlayerMovement Error: Missing component");
        }
        
        // Setting max parameters of rigidbody
        _myBody.maxLinearVelocity = maxLinearVelocity;
        _myBody.maxAngularVelocity = maxAngularVelocity;

        _startingPosition = transform.position;
        _startingRotation = transform.rotation;

        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
        
        Initialize();
    }

    private void Initialize() {
        if (disableOnStart) gameObject.SetActive(false);
        
        transform.position = _startingPosition;
        transform.rotation = _startingRotation;
        
        _myBody.velocity = Vector3.zero;
        _myBody.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Alpha0)) {
            Initialize();
        }

        if (acceptingInputs)
        {
            _turnVal = Input.GetAxis("Turn") * turnForce;

            _accelVal = Input.GetAxis("Accelerate");
            
            // Input value between 0 and -1 if decelerating
            if (_accelVal > 0f) _accelVal *= accelForce;
        
            _jumpRequest = _jumpRequest || Input.GetKeyDown(KeyCode.Space);
        
            _strafeLeft = _strafeLeft || Input.GetKeyDown(KeyCode.Q);
            _strafeRight = _strafeRight || Input.GetKeyDown(KeyCode.E);
        }
        else
        {
            _turnVal = 0f;
            _accelVal = 0f;
            _jumpRequest = false;
            _strafeLeft = false;
            _strafeRight = false;
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
        if (Mathf.Abs(_turnVal) > Mathf.Epsilon) Turn(_turnVal);
        
        if (Mathf.Abs(_accelVal) > Mathf.Epsilon) Accelerate(_accelVal);
        
        if (_strafeLeft || _strafeRight) Strafe();
        
        if (_jumpRequest) Jump();
    }
    
    #endregion

    #region Movement Functions
    
    // turnVal is amount to turn, -1 for max left, 1 for max right
    public void Turn(float turnVal)
    {
        Vector3 linForce = (_camInvRot * _myCam.right) * turnVal;
        Vector3 rotForce = (_camInvRot * _myCam.up) * turnVal;

        // Linear acceleration
        if (isIcy()) linForce /= slipperyForce;
        _myBody.AddForce(linForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotForce, ForceMode.Impulse);
    }
    
    // accelVal is amount to speed up or down, -1 max decelerating, 1 max accelerating
    public void Accelerate(float accelVal)
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
            linearForce = camForward * accelVal;
            rotationalForce = camRight * accelVal;
        }

        // Skid if on ice (reduce linear acceleration)
        if (isIcy()) linearForce /= slipperyForce;
        
        // Linear acceleration
        _myBody.AddForce(linearForce, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque(rotationalForce, ForceMode.Impulse);
    }
    
    /* To do: Make strafe stabilize after some amount of distance (Maybe use Rigidbody Move method? */
    private void Strafe()
    {
        if (_strafeRight) _myBody.AddForce((_camInvRot * _myCam.right) * strafeForce, ForceMode.Impulse);
        if (_strafeLeft) _myBody.AddForce((_camInvRot * _myCam.right) * (-1 * strafeForce), ForceMode.Impulse);

        _strafeRight = false;
        _strafeLeft = false;
    }

    // Check y-axis velocity for jump being legal
    private void Jump()
    {
        if (Grounded())
        {
            _myBody.AddForce((_camInvRot * _myCam.up) * jumpForce, ForceMode.Impulse);
        }

        _jumpRequest = false;
    }


    // Adds spin to the ball
    // Positive spinVal spins CW (right), negative spins CCW (left)
    // Magnitude of spinVal determines magnitude of torque
    public void Spin(float spinVal)
    {
        _myBody.AddTorque((_camInvRot * _myCam.up) * spinVal, ForceMode.Impulse);
    }

    
    #endregion

    #region Support Functions
    
    // Returns true if on the ground, false otherwise
    /* Will need to be updated if we ever add curved terrain with different normals */
    private bool Grounded()
    {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, _myCollider.radius, (_camInvRot * _myCam.up) * -1, out hit,
            1f, LayerMask.GetMask("Ground"));
    }

    //Returns true is on icy ground, false if not
    /* same as above, if we add slopes, needs to be updated*/
    public bool isIcy()
    {
        RaycastHit detectIce;
        return Physics.SphereCast(transform.position, _myCollider.radius, (_camInvRot * _myCam.up) * -1, out detectIce,
            1f, LayerMask.GetMask("Icy Ground"));
    }
    
    #endregion
}
