using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Object Variables
    
    private Rigidbody _myBody;
    private Transform _myCam;
    private Quaternion _camInvRot;
    private SphereCollider _myCollider;
    
    #endregion

    #region Action Flags
    
    private bool _jumpRequest;
    private bool _strafeLeft;
    private bool _strafeRight;
    
    #endregion
    
    [Header("Force specifications")]
    [SerializeField] private float accelForce;
    [SerializeField] private float turnForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float strafeForce;
    
    [Header("Rigidbody configuration")]
    [SerializeField] private float maxLinearVelocity = 1e+16f;
    [SerializeField] private float maxAngularVelocity = 50f;

    #region MonoBehaviour Functions
    
    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        
        _myCam = GameObject.FindWithTag("MainCamera").transform;
        _camInvRot = Quaternion.Inverse(_myCam.rotation);
        
        _myCollider = GetComponent<SphereCollider>();

        if (!_myBody || !_myCam || !_myCollider)
        {
            Debug.LogError("PlayerMovement Error: Missing component");
        }
        
        // Setting max parameters of rigidbody
        _myBody.maxLinearVelocity = maxLinearVelocity;
        _myBody.maxAngularVelocity = maxAngularVelocity;
    }

    private void Update()
    {
        _jumpRequest = _jumpRequest || Input.GetKeyDown(KeyCode.Space);
        
        _strafeLeft = _strafeLeft || Input.GetKeyDown(KeyCode.Q);
        _strafeRight = _strafeRight || Input.GetKeyDown(KeyCode.E);
    }
    
    private void FixedUpdate()
    {
        Turn();
        
        Accelerate();
        
        if (_strafeLeft || _strafeRight) Strafe();
        
        if (_jumpRequest) Jump();
    }
    
    #endregion

    #region Movement Functions
    
    private void Turn()
    {
        // Amount to turn, -1 for max left, 1 for max right
        float turnVal = Input.GetAxis("Turn") * turnForce;
        
        // Linear acceleration
        _myBody.AddForce((_camInvRot * _myCam.right) * turnVal, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque((_camInvRot * _myCam.up) * turnVal, ForceMode.Impulse);
    }

    private void Accelerate()
    {
        // Amount to speed up or down, -1 max decelerating, 1 max accelerating
        float accelVal = Input.GetAxis("Accelerate") * accelForce;
        
        // Linear acceleration
        _myBody.AddForce((_camInvRot * _myCam.forward) * accelVal, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque((_camInvRot * _myCam.right) * accelVal, ForceMode.Impulse);
    }
    
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
    
    #endregion
}
