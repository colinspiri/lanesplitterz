using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Movement")]
    [TaskDescription("Move the cannon by some angle")]
    
    public class RotateCannon : Action
    {
        [Tooltip("Pick random angle from range?")]
        public SharedBool randomize = false;
        
        // Rotation ranges to use if randomized
        public SharedFloat minX;
        public SharedFloat maxX;
        public SharedFloat minY;
        public SharedFloat maxY;
        
        // Rotations to use if not randomized
        public SharedFloat xRot;
        public SharedFloat yRot;
        
        private EnemyCannon _myCannon;
        private bool _startedRotation;
        
        public override void OnStart()
        {
            _myCannon = gameObject.GetComponentInChildren<EnemyCannon>(true);

            _startedRotation = false;
        }

        public override TaskStatus OnUpdate()
        {
            // No cannon found
            if (!_myCannon)
            {
                return TaskStatus.Failure;
            }
            // Rotation has started
            else if (_startedRotation)
            {
                // Rotation is in progress
                if (_myCannon.Rotating)
                {
                    return TaskStatus.Running;
                }
                // Rotation is finished
                else
                {
                    return TaskStatus.Success;
                }
            }
            // Waiting for another rotation to finish
            else if (_myCannon.Rotating)
            {
                return TaskStatus.Running;
            }
            // Begin randomized rotation
            else if (randomize.Value)
            {
                _myCannon.Rotate(Random.Range(minX.Value, maxX.Value), Random.Range(minY.Value, maxY.Value));

                _startedRotation = true;

                return TaskStatus.Running;
            }
            // Begin rotation
            else
            {
                _myCannon.Rotate(xRot.Value, yRot.Value);
                
                _startedRotation = true;

                return TaskStatus.Running;
            }
        }
        
    }
}


