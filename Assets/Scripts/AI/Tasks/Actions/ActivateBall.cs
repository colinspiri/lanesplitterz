using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Movement")]
    [TaskDescription("Sets the destination of the ball's AI")]
    
    public class ActivateBall : Action
    {
        public SharedTransform destination;
        
        private NavMeshAgent _myAgent;
        private EnemyBall _myBall;
        
        public override void OnStart()
        {
            _myAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
            _myAgent.isStopped = false;

            _myBall = gameObject.GetComponentInChildren<EnemyBall>();

            if (destination.Value)
            {
                _myAgent.SetDestination(destination.Value.position);
            }
            else
            {
                _myBall.SetDestination();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_myAgent.pathPending)
            {
                return TaskStatus.Running;
            }
            else if (_myAgent.isPathStale || !_myAgent.hasPath || _myAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
        }
        
    }
}