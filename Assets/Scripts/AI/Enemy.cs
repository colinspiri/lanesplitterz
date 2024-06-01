using BehaviorDesigner.Runtime;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorTree _myTree;
    public bool Launched { get; set; }
    
    private void Start()
    {
        _myTree = GetComponent<BehaviorTree>();
    }

    public void LaunchSequence()
    {
        _myTree.enabled = true;
    }
}
