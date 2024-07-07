using BehaviorDesigner.Runtime;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorTree _myTree;
    public bool Launched { get; set; }

    private void Start()
    {
        _myTree = GetComponent<BehaviorTree>();

        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
    }

    private void Initialize()
    {
        _myTree.SetVariable("launched", (SharedBool) false);
        Launched = false;
        _myTree.enabled = false;
    }

    public void LaunchSequence()
    {
        _myTree.enabled = true;
    }
}
