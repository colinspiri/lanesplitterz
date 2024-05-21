using System;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {
    public static CameraFollowObject Instance;
    
    public GameObject objectToFollow;

    [SerializeField] private float moveSpeed;

    private Vector3 _targetPosition;
    private Vector3 _offset;
    private bool _followEnabled;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - objectToFollow.transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (_followEnabled) {
            LerpTowardsObject();
        }
    }

    private void LerpTowardsObject() {
        if (objectToFollow != null) {
            _targetPosition = objectToFollow.transform.position + _offset;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
    }

    public void EnableFollow() {
        _followEnabled = true;
    }
}