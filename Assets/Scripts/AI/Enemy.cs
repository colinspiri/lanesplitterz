using BehaviorDesigner.Runtime;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorTree _myTree;
    private Rigidbody _myBallBody;
    private EnemyBall _myBall;
    private EnemyCannon _myCannon;
    public bool Launched { get; set; }

    private void Start()
    {
        _myTree = GetComponent<BehaviorTree>();
        _myBallBody = GetComponentInChildren<Rigidbody>(true);
        _myBall = GetComponentInChildren<EnemyBall>(true);
        _myCannon = GetComponentInChildren<EnemyCannon>(true);

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
    }

    public void LaunchSequence()
    {
        _myTree.enabled = true;
    }
}
