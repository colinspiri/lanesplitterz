using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBall : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform defaultDestination;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        gameObject.SetActive(false);
    }

    public void SetDestination()
    {
        _agent.SetDestination(defaultDestination.position);
    }
}
