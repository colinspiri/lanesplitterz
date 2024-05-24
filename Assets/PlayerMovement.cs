using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Object variables
    
    private Rigidbody _myBody;
    private Transform _myCam;
    
    #endregion
    
    [Header("Force specifications")]
    // Max amount of force to apply for acceleration
    [SerializeField] private float accelForce;
    
    [Header("Rigidbody configuration")]
    [SerializeField] private float maxLinearVelocity = 1e+16f;
    [SerializeField] private float maxAngularVelocity = 50f;

    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        _myCam = GameObject.FindWithTag("MainCamera").transform;

        if (!_myBody)
        {
            Debug.LogError("PlayerMovement Error: Missing rigidbody");
        }
        
        if (!_myCam)
        {
            Debug.LogError("PlayerMovement Error: Missing main camera");
        }
        
        // Setting max parameters of rigidbody
        _myBody.maxLinearVelocity = maxLinearVelocity;
        _myBody.maxAngularVelocity = maxAngularVelocity;
    }
    private void FixedUpdate()
    {
        Turn();
        
        Accelerate();
        
        Strafe();
        
        Jump();
    }

    private void Turn()
    {
        // Amount to turn, -1 for max left, 1 for max right
        float turnVal = Input.GetAxis("Turn");
    }

    private void Accelerate()
    {
        // Amount to speed up or down, -1 max decelerating, 1 max accelerating
        float accelVal = Input.GetAxis("Accelerate") * accelForce;
        
        // Linear acceleration
        _myBody.AddForce(_myCam.forward * accelVal, ForceMode.Impulse);
        
        // Rotational acceleration
        _myBody.AddTorque(_myCam.right * accelVal, ForceMode.Impulse);
    }
    
    private void Strafe()
    {
        // Whether to strafe left and / or right
        bool strafeLeft = Input.GetKeyDown(KeyCode.Q);
        bool strafeRight = Input.GetKeyDown(KeyCode.E);
    }

    // Check y-axis velocity for jump being legal
    private void Jump()
    {
        // Whether to jump on this frame
        bool jump = Input.GetKeyDown(KeyCode.Space);
    }
}
