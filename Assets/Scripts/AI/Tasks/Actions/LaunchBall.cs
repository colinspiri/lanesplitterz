using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Movement")]
    [TaskDescription("Fires the ball out of the cannon")]
    
    public class LaunchBall : Action
    {
        public enum LaunchTypes
        {
            LaunchDefault,
            LaunchInRange,
            LaunchDirect
        }

        public LaunchTypes launchType;
        
        // Parameters for launch range
        public SharedFloat forceMin;
        public SharedFloat forceMax;
        
        // Parameter for direct launch
        public SharedFloat force;
        
        private EnemyCannon _myCannon;
        
        public override void OnStart()
        {
            _myCannon = gameObject.GetComponentInChildren<EnemyCannon>();
        }

        public override TaskStatus OnUpdate()
        {
            switch (launchType)
            {
                case (LaunchTypes.LaunchDefault):
                    _myCannon.LaunchDefault();
                    
                    return TaskStatus.Success;
                    break;
                case (LaunchTypes.LaunchInRange):
                    _myCannon.LaunchInRange(forceMin.Value, forceMax.Value);
                    
                    return TaskStatus.Success;
                    break;
                case (LaunchTypes.LaunchDirect):
                    _myCannon.Launch(force.Value);
                    
                    return TaskStatus.Success;
                    break;
                default:
                    Debug.LogError("LaunchBall Error: Invalid launch type specified");

                    return TaskStatus.Failure;
                    break;
            }
        }
        
    }
}