using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {
    [SerializeField] private GameObject _objectToFollow;
    private Transform _myCamera;
    
    [SerializeField] private float moveSpeed;

    private Vector3 _targetPosition;
    private Vector3 _offset;

    private Vector3 _initialPos;
    private Quaternion _initialRot;

    private Vector3 _initialCamPos;
    private Quaternion _initialCamRot;

    private bool _initialized = false;

    // Start is called before the first frame update
    void Start() {
        // _objectToFollow = PlayerMovement.Instance.gameObject;
        _offset = transform.position - _objectToFollow.transform.position;
        _myCamera = transform.GetChild(0);

        _initialCamPos = _myCamera.position;
        _initialCamRot = _myCamera.rotation;

        _initialPos = transform.position;
        _initialRot = transform.rotation;

        _initialized = true;
    }

    // Update is called once per frame
    void Update() {
        LerpTowardsObject();
    }

    private void LerpTowardsObject() {
        if (_objectToFollow != null) {
            _targetPosition = _objectToFollow.transform.position + _offset;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
    }

    public void ResetForward(Vector3 newForward)
    {
        Vector3 xzForward = transform.forward;
        xzForward.y = 0f;
        newForward.y = 0f;
        
        float angleDiff = Vector3.SignedAngle(xzForward, newForward, Vector3.up);
        Quaternion diffRot = Quaternion.AngleAxis(angleDiff, Vector3.up);

        // Rotate position around target
        // Let the existing LERP system smooth this out
        _offset = diffRot * _offset;

        // Rotate orientation gradually
        Quaternion destRot = diffRot * transform.rotation;
        StartCoroutine(RotateOrientation(destRot));
    }

    public void DefaultForward()
    {
        if (!_initialized) return;
        
        transform.position = _initialPos;
        transform.rotation = _initialRot;
        _offset = transform.position - _objectToFollow.transform.position;

        _myCamera.position = _initialCamPos;
        _myCamera.rotation = _initialCamRot;

        //Debug.Log("Camera Printout:");
        //Debug.Log("Camera World Position is " + _myCamera.position);
        //Debug.Log("Camera Local Position is " + _myCamera.localPosition);
        //Debug.Log("Camera World Rotation is " + _myCamera.rotation);
        //Debug.Log("Camera Local Rotation is " + _myCamera.localRotation);
    }

    private IEnumerator RotateOrientation(Quaternion destRot)
    {
        while (Quaternion.Angle(transform.rotation, destRot) > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, destRot, moveSpeed * Time.deltaTime);
            
            yield return new WaitForEndOfFrame();
        }
    }
}