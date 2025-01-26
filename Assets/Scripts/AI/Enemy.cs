using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using Yarn.Unity;

public class Enemy : MonoBehaviour
{
    private BehaviorTree _myTree;
    private Rigidbody _myBallBody;
    private EnemyBall _myBall;
    private EnemyCannon _myCannon;
    private Transform _myCannonObj;
    private EnemyRotation _myRotation;
    private Vector3 _ogPosition;
    private Vector3 _ogScale;
    private float _ogGravity;
    private float _ogSelfForce;
    private float _ogPlayerForce;

    private Quaternion _startCannonRot;
    
    public bool Launched { get; set; }

    private void Start()
    {
        _ogPosition = transform.position;
        _ogScale = transform.localScale;

        _myTree = GetComponent<BehaviorTree>();
        
        _myBallBody = GetComponentInChildren<Rigidbody>(true);
        
        _myBall = GetComponentInChildren<EnemyBall>(true);

        _ogGravity = _myBall.extraGravity;

        _ogSelfForce = _myBall.selfCollisionForce;
        _ogPlayerForce = _myBall.playerCollisionForce;


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
        _myBall.NewThrow();
        _myBallBody.Sleep();
        _myBallBody.gameObject.SetActive(false);
        _myBallBody.transform.position = _myCannon.LaunchPoint.position;
        _myBallBody.constraints = RigidbodyConstraints.None;

        _myCannonObj.transform.rotation = _startCannonRot;
    }

    [YarnCommand("Embiggen")]

    public void Embiggen()
    {
        transform.Translate(new Vector3(2.5f, 2.68f, 0f));
        transform.localScale *= 2f;
        _myBall.extraGravity *= 2f;
        _myBall.selfCollisionForce /= 2f;
        _myBall.playerCollisionForce *= 2f;
    }

    [YarnCommand("Ensmallen")]

    public void Ensmallen()
    {
        transform.position = _ogPosition;
        transform.localScale = _ogScale;
        _myBall.extraGravity = _ogGravity;
        _myBall.selfCollisionForce = _ogSelfForce;
        _myBall.playerCollisionForce = _ogPlayerForce;
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
