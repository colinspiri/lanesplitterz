using System;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {
    private GameObject _objectToFollow;

    [SerializeField] private float moveSpeed;

    private Vector3 _targetPosition;
    private Vector3 _offset;

    // Start is called before the first frame update
    void Start() {
        _objectToFollow = PlayerMovement.Instance.gameObject;
        _offset = transform.position - _objectToFollow.transform.position;
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
}