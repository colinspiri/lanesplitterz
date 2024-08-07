using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorTree _myTree;
    private Rigidbody _myBallBody;
    private EnemyBall _myBall;
    private EnemyCannon _myCannon;
    private Transform _myCannonObj;
    private EnemyRotation _myRotation;

    private Quaternion _startCannonRot;
    
    public bool Launched { get; set; }

    private void Start()
    {
        _myTree = GetComponent<BehaviorTree>();
        
        _myBallBody = GetComponentInChildren<Rigidbody>(true);
        
        _myBall = GetComponentInChildren<EnemyBall>(true);
        
        _myCannon = GetComponentInChildren<EnemyCannon>(true);
        _myCannonObj = _myCannon.transform;
        _startCannonRot = _myCannonObj.rotation;
        
        _myRotation = GetComponentInChildren<EnemyRotation>(true);

        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
    }

    private void Initialize()
    {
        _myTree.SetVariable("launched", (SharedBool) false);
        Launched = false;
        _myTree.enabled = false;

        _myBall.RestoreFuel(1f);
        _myBallBody.Sleep();
        _myBallBody.gameObject.SetActive(false);
        _myBallBody.transform.position = _myCannon.LaunchPoint.position;
        _myBallBody.constraints = RigidbodyConstraints.None;

        _myCannonObj.transform.rotation = _startCannonRot;
    }

    public void LaunchSequence()
    {
        _myTree.enabled = true;
    }
    
    public void ResetForward(Vector3 newForward)
    {
        _myRotation.ResetForward(newForward);
    }
}
