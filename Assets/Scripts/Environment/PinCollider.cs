using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCollider : MonoBehaviour
{
    [SerializeField] private Rigidbody parentBody;
    private Rigidbody _myBody;

    private Vector3 _lastLinVelocity;
    private Vector3 _lastAngVelocity;
    // private SphereCollider _myCollider;
    
    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        // _myCollider = GetComponent<SphereCollider>();

        _myBody.mass = parentBody.mass;
        _myBody.drag = parentBody.drag;
        _myBody.angularDrag = parentBody.angularDrag;
        _lastLinVelocity = parentBody.velocity;
        _lastAngVelocity = parentBody.angularVelocity;
    }
    

    private void FixedUpdate()
    {
        _myBody.position = parentBody.position;
        // _myBody.rotation = parentBody.rotation;
        // _myBody.velocity = parentBody.velocity;
        // _myBody.angularVelocity = parentBody.angularVelocity;
        // _myBody.inertiaTensor = parentBody.inertiaTensor;
        // _myBody.inertiaTensorRotation = parentBody.inertiaTensorRotation;


        // Add linear force to match change in parent linear acceleration
        // F = ma, F = mdv/dt
        // Fdt = mdv
        Vector3 velocityDiff = parentBody.velocity - _lastLinVelocity;
        // Debug.Log("PinCollider: Linear Velocity Diff is " + velocityDiff.magnitude);
        _myBody.AddForce(parentBody.mass * velocityDiff, ForceMode.Impulse);
        
        _lastLinVelocity = parentBody.velocity;
        
        // Add rotational force to match change in parent linear acceleration
        // T = Ia, T = Idv/dt
        // Tdt = Idv
        velocityDiff = parentBody.angularVelocity - _lastAngVelocity;
        // Debug.Log("PinCollider: Angular Velocity Diff is " + velocityDiff.magnitude);
        Vector3 inertia = parentBody.inertiaTensorRotation * parentBody.inertiaTensor;
        Vector3 Idv = MatVecProduct(inertia, velocityDiff);
        _myBody.AddTorque(Idv, ForceMode.Impulse);
        
        _lastAngVelocity = parentBody.angularVelocity;
    }

    // Treats 'diagonal' as the diagonal values of a diagonal 3x3 matrix
    private Vector3 MatVecProduct(Vector3 diagonal, Vector3 vec)
    {
        return new Vector3(diagonal.x * vec.x, diagonal.y * vec.y, diagonal.z * vec.z);
    }

    // Disable collision after initial hit to remove excess force
    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Pins"))
    //     {
    //         Physics.IgnoreCollision(_myCollider, collision.collider, true);
    //     }
    // }
}
